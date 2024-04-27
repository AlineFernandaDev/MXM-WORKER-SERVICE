using EmailDispatching;
using EmailDispatching.Interfaces;
using EmailDispatching.Models;
using EmailDispatching.Services;
IConfiguration configuration = new ConfigurationBuilder()

   .SetBasePath(Directory.GetCurrentDirectory())

   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)

   .Build();


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
        services.Configure<EmailSettings>(configuration.GetSection("SmptConfig"));
    })
    .Build();


await host.RunAsync();


