using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using EmailDispatching.Repositories.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace EmailDispatching.Repositories.Services
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _connectionFactory;
        private IModel _channel;
        private readonly EmailSettings _emailSettings;

        public RabbitMQConnection(IConfiguration configuration,IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _configuration = configuration;
            _connectionFactory = new ConnectionFactory()
            {
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"],
                HostName = _configuration["RabbitMQ:HostName"],
                VirtualHost = _configuration["RabbitMQ:VirtualHost"],
                Port = Convert.ToInt32(_configuration["RabbitMQ:Port"]),
            };
            _channel = CreateModel();
        }

        public async Task<string> ConsumerRabbitMQ(string queueName)
        {
            try
            {
                _channel.QueueDeclarePassive(queueName);

                string stringObjetcReturn = "";

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    stringObjetcReturn = Encoding.UTF8.GetString(body);


                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                _channel.BasicConsume(queue: queueName,
                                      autoAck: false,
                                      consumer: consumer);

                await Task.Delay(Timeout.Infinite);

                return stringObjetcReturn;
            }
            catch (Exception ex)
            {
                return $"Error starting RabbitMQ connection: {ex.Message}";
            }
        }

        public IModel CreateModel()
        {
            var connection = _connectionFactory.CreateConnection();
            return connection.CreateModel();
        }

        public async Task<string>ProcessarEmails()
        {
            var message = await ConsumerRabbitMQ("queueSendEmails");
            return message;

            Console.WriteLine($"Mensagem recebida: {message}");
            // if (message == null)
            // {
            //     return;
            // };
        var email = JsonConvert.DeserializeObject<SendEmail>(message);
        var sendMessage = new MailMessage{
            From = new MailAddress(_emailSettings.EmailFromAddress),
            Subject = email.Nome,
            Body = email.Corpo,
            IsBodyHtml = true,
        };
        sendMessage.To.Add(email.EnderecoDestino);
            using (var client = new SmtpClient(_emailSettings.ServerSmtp, _emailSettings.Port))
            {
               try{
                client.Credentials = new System.Net.NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                client.EnableSsl = true;
                await client.SendMailAsync(sendMessage);
                return $"E-mail enviado para {email.EnderecoDestino} com sucesso.";
                }
                catch (Exception ex)
                {
                    return$"Erro ao enviar e-mail: {ex.Message}";
                    
                }
            }
        }
        public class SendEmail
        {
            public string Nome { get; set; } = null!;
            public string EnderecoDestino { get; set; } = null!;
            public string Corpo { get; set; } = null!;
        }
        public class EmailSettings
        {
            public string ServerSmtp { get; set; } = null!;
            public int Port { get; set; }
            public string UserName { get; set; } = null!;
            public string Password { get; set; } = null!;
            public bool EnableSsl { get; set; }
            public string EmailFromAddress { get; set; } = null!;
        }
    }
}
