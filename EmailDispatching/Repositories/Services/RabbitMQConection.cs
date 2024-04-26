using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using EmailDispatching.Repositories.Contracts;

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
            _channel = CreateChannel();
        }

        public async Task<string> RabbitMQ(string queueName)
        {
            try
            {
                // Criar conexão e começar a consumir mensagens
                await CreateConnection(queueName);
                return "RabbitMQ connection started successfully";
            }
            catch (Exception ex)
            {
                return $"Error starting RabbitMQ connection: {ex.Message}";
            }
        }

        private async Task CreateConnection(string queueName)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: true, autoDelete: false, arguments: null);

            // Configurar o consumidor para receber e-mails da fila
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await SendEmail(message); //TODO: SERIALIZAR E ENVIAR COMO E-MAIL
            };
            // Iniciar o consumo da fila
            await Task.Run(() => _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer));
        }

        public IModel CreateChannel()
        {
            var connection = _connectionFactory.CreateConnection();
            return connection.CreateModel();
        }

        public async Task SendEmail(string message)
        {
            // Lógica para processar a mensagem e enviar o e-mail
            var parts = message.Split('|'); // Supõe que a mensagem está no formato "to|subject|body"
            string to = parts[0];
            string subject = parts[1];
            string body = parts[2];

            // Configuração do provedor de e-mail (substitua com suas próprias credenciais e servidor SMTP)
            var smtpServer = _configuration["EmailSettings:Mail"];
            var smtpPort = Convert.ToInt32(_configuration["EmailSettings:Port"]);
            var username = _configuration["EmailSettings:UserName"];
            var password = _configuration["EmailSettings:Password"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(username, password);
                client.EnableSsl = true;

                var mail = new MailMessage(username, to, subject, body);
                await client.SendMailAsync(mail);
            }

            Console.WriteLine($"E-mail enviado para {to}");
        }

        public IRabbitMQConnection? RabbitMQ(object queueName)
        {
            throw new NotImplementedException();
        }
    }
}
