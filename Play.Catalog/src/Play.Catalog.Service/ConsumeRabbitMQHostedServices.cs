using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Play.Catalog.Service
{
    public class ConsumeRabbitMQHostedServices : BackgroundService
    {
        private readonly ILogger<ConsumeRabbitMQHostedServices> _logger;

        public ConsumeRabbitMQHostedServices(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedServices>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "saleitems-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation(" [*] Waiting for messages. To exit press CTRL+C");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($" [x] Received {message}");
                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: "saleitems-queue",
                autoAck: true,
                consumer: consumer
            );

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
