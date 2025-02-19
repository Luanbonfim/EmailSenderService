using EmailSenderService;
using ProductEmailService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<RabbitMQSettings>(context.Configuration.GetSection("RabbitMQ"));

        services.Configure<EmailSettings>(context.Configuration.GetSection("EmailSettings"));

        services.AddSingleton<RabbitMQConsumer>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();