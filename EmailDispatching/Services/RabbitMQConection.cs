using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using EmailDispatching.Models;
using EmailDispatching.Interfaces;

namespace EmailDispatching.Services
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly RabbitMQConfiguration _rabbitMQconfiguration;
        private IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private IModel _channel;
        private readonly EmailSettings _emailSettings;

        public RabbitMQConnection(IOptions<EmailSettings> emailSettings, IOptions<RabbitMQConfiguration> rabbitMQconfiguration)
        {
            _emailSettings = emailSettings.Value;
            _connectionFactory = new ConnectionFactory()
            {
                UserName = _rabbitMQconfiguration.UserName,
                Password = _rabbitMQconfiguration.Password,
                HostName = _rabbitMQconfiguration.HostName,
                VirtualHost = _rabbitMQconfiguration.VirtualHost,
                Port = _rabbitMQconfiguration.Port,
            };
            _channel = CreateModel();
        }
        public async Task<string> ConsumerRabbitMQ(string queueName)
        {
            try
            {
                string stringObjectReturn = "";
                _channel.QueueDeclarePassive(queueName);
                var consumer = new EventingBasicConsumer(_channel);
                BasicGetResult result = _channel.BasicGet(queueName, autoAck: true);
                if (result == null) return "";
                var bodyBytes = result.Body.ToArray();
                stringObjectReturn = Encoding.UTF8.GetString(bodyBytes);
                return stringObjectReturn;
            }
            catch (Exception ex)
            {
                return $"Error starting RabbitMQ connection: {ex.Message}";
            }
        }
        public IModel CreateModel()
        {
            try
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    _connection = _connectionFactory.CreateConnection();
                    _channel = _connection.CreateModel();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return _channel;
        }

        public async Task<string> EmailProcess()
        {
            var message = await ConsumerRabbitMQ("queueSendEmails");
            if (message == "")
            {
                return "Não Existe E-mails a serem enviados";
            };
            var email = JsonConvert.DeserializeObject<SendMail>(message);
            var sendMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.EmailFromAddress),
                Subject = email.Nome,
                Body = email.Corpo,
                IsBodyHtml = true,
            };
            sendMessage.To.Add(email.EnderecoDestino);
            using (var client = new SmtpClient(_emailSettings.ServerSmtp, _emailSettings.Port))
            {
                try
                {
                    client.Credentials = new System.Net.NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                    client.EnableSsl = true;
                    await client.SendMailAsync(sendMessage);
                    return $"E-mail enviado para {email.EnderecoDestino} com sucesso.";
                }
                catch (Exception ex)
                {
                    return $"Erro ao enviar e-mail: {ex.Message}";

                }
            }

        }
    }
}
