using System.Text;
using System.Text.Json;
using DnsClient.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.RabbitMQ.ConsumerContracts;
using OrdersMicroservice.Core.RabbitMQ.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrdersMicroservice.Core.RabbitMQ.Consumers
{
    public class RabbitMQProductNameUpdateConsumer : IRabbitMQProductNameUpdateConsumer
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private readonly ILogger<RabbitMQProductNameUpdateConsumer> _logger;
        public RabbitMQProductNameUpdateConsumer(IConfiguration configuration, ILogger<RabbitMQProductNameUpdateConsumer> logger)
        {
            _configuration = configuration;
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RABBITMQ_HOST"]!,
                UserName = configuration["RABBITMQ_USERNAME"]!,
                Password = configuration["RABBITMQ_PASSWORD"]!,
                Port = Convert.ToInt32(configuration["RABBITMQ_PORT"]!)
            };
            _logger = logger;
        }
        public async Task ConsumeAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }
            var channel = await _connection.CreateChannelAsync();

            string routeKey = "product.update.name";
            string queueName = "orders.update.name.queue";

            string exchangeName = _configuration["RABBITMQ_PRODUCTS_EXCHANGE"]!;
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
            );
            // create a message queue if it doesn't exist
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            // bind the queue to the exchange with the route key
            await channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routeKey
            );

            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(args.Body.ToArray());
                    var productUpdate = JsonSerializer.Deserialize<ProductNameUpdateMessage>(message);
                    _logger.LogInformation("Received product name update message: {Message}", message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing product name update message");
                }
            };
            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
        }
        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
