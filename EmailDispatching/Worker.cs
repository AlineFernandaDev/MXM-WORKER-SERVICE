using EmailDispatching.Interfaces;
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
                var result = await ValidateEmail();
                _logger.LogInformation("Result from RabbitMQConnection: {result}", result);

                await Task.Delay(1000, stoppingToken);
            }
        }
        private async Task<string> ValidateEmail()
        {
            var result = await _rabbitMQConnection.EmailProcess();
            return result;
        }
    }
}

