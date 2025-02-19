using ProductEmailService;

namespace EmailSenderService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQConsumer _rabbitMQConsumer;

        public Worker(ILogger<Worker> logger, RabbitMQConsumer rabbitMQConsumer)
        {
            _logger = logger;
            _rabbitMQConsumer = rabbitMQConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service started.");
            _rabbitMQConsumer.StartConsuming(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("Worker service stopped.");
        }
    }
}