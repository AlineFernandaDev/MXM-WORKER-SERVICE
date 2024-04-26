using EmailDispatching;
using EmailDispatching.Repositories.Contracts;
using EmailDispatching.Repositories.Services;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();

        ;
        
    })
    
    
    .Build();

    IConfiguration configuration = new ConfigurationBuilder()

    .SetBasePath(Directory.GetCurrentDirectory())

    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    
    .Build();




await host.RunAsync();


//TODO:ver o chat gpt e configurar as injecoes de dependencia na worker e da worker extender na Program, Não esquece daquela configuração do vigia da fila