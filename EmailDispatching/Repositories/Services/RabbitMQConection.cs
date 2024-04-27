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

namespace EmailDispatching.Repositories.Services
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _connectionFactory;
        private IModel _channel;

        public RabbitMQConnection(IConfiguration configuration)
        {
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

        public async Task<string> RabbitMQ(string queueName)
        {
            try
            {
                _channel.QueueDeclare(queue: "queueSendEmails", durable: true, exclusive: false, autoDelete: false, arguments: null);
                if (_channel == null || _channel.IsClosed)
                {
                    _channel = CreateModel();
                }
                await ProcessarEmails("queueSendEmails");
                return "RabbitMQ connection started successfully";
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

        private async Task ProcessarEmails(string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, ea) =>
            {
                var objeto = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(objeto);

                var email = JsonConvert.DeserializeObject<EmailMessage>(message);


                var Host = _configuration["EmailSettings:smtpHost"];
                var Port = Convert.ToInt32(_configuration["EmailSettings:smtpPort"]);
                var UsernameEmail = _configuration["EmailSettings:UsernameEmail"];
                var UsernamePassword = _configuration["EmailSettings:UsernamePassword"];

                using (var client = new SmtpClient(Host, Port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(UsernameEmail, UsernamePassword);
                    client.EnableSsl = true;

                    try
                    {
                        var mail = new MailMessage()
                        {
                            From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                            To = { email.To },
                            Subject = email.Subject,
                            Body = email.Body
                        };

                        await client.SendMailAsync(mail);

                        Console.WriteLine($"E-mail enviado para {mail.To} com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
                    }
                }
            };

            _channel.BasicConsume(queue: "queueSendEmails", autoAck: false, consumer: consumer);
            await Task.CompletedTask;
        }
        public class EmailMessage
        {
            public string From { get; set; } = null!;
            public string To { get; set; } = null!;
            public string Subject { get; set; } = null!;
            public string Body { get; set; } = null!;
        }
    }
}
