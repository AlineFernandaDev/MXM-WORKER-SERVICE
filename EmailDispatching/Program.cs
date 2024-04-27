using EmailDispatching;
using EmailDispatching.Repositories.Contracts;
using EmailDispatching.Repositories.Services;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<IRabbitMQConnection, RabbitMQConnection>();
        
    })
    
    
    .Build();

    IConfiguration configuration = new ConfigurationBuilder()

    .SetBasePath(Directory.GetCurrentDirectory())

    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    
    .Build();




await host.RunAsync();


