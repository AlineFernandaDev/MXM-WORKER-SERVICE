using EmailDispatching.Repositories.Contracts;
namespace EmailDispatching
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMQConnection _rabbitMQConnection;

        public Worker(ILogger<Worker> logger, IRabbitMQConnection rabbitMQConnection)
        {
            _logger = logger;
            _rabbitMQConnection = rabbitMQConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // Chamar o método RabbitMQConnection passando o nome da fila
                string result = await RabbitMQ("queueName");

                // Você pode usar o resultado aqui, se necessário
                _logger.LogInformation("Result from RabbitMQConnection: {result}", result);

                await Task.Delay(1000, stoppingToken);
            }
        }
        private async Task<string> RabbitMQ(string queueName)
        {
            string message = await _rabbitMQConnection.RabbitMQ(queueName);
            return message;
        }
    }
}
