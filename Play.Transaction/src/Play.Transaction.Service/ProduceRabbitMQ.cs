using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Play.Transaction.Service;

class ProduceRabbitMQ
{
    static async Task Main(string[] args)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class with the specified AMQP URI.
        /// The URI includes the protocol, username, password, host, and port for connecting to the RabbitMQ server.
        /// </summary>
        var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            "hello",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        const string message = "Hello World";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);

        Console.WriteLine($" [x] Sent {message}");
        Console.WriteLine($" Press [enter] to exit");
        Console.ReadLine();
    }
}
