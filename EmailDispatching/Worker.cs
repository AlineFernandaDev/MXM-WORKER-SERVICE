using EmailDispatching.Repositories.Contracts;
namespace EmailDispatching
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)

            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var result = await ValidarEmail();
                _logger.LogInformation("Result from RabbitMQConnection: {result}", result);

                await Task.Delay(1000, stoppingToken);
            }
        }
    private async Task<string>ValidarEmail(){
        using(var scope = _serviceProvider.CreateScope()){
            var _rabbitMQConnection = scope.ServiceProvider.GetRequiredService<IRabbitMQConnection>();
            var result = await _rabbitMQConnection.ProcessarEmails();
        return result;
        }
       
    }
}
}

