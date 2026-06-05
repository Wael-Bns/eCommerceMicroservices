
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.RabbitMQ.ConsumerContracts;
using OrdersMicroservice.Core.RabbitMQ.Messages;
using OrdersMicroservice.Core.RabbitMQ.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrdersMicroservice.Core.RabbitMQ.Consumers
{
    public class RabbitMQDeleteProductConsumer : IRabbitMQDeleteProductConsumer
    {
        private readonly ILogger<RabbitMQDeleteProductConsumer> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;
        private IConnection? _connection;
        public RabbitMQDeleteProductConsumer(IConfiguration configuration, ILogger<RabbitMQDeleteProductConsumer> logger, IDistributedCache distributedCache)
        {
            _configuration = configuration;
            _logger = logger;
            _distributedCache = distributedCache;
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration[RabbitMqNamings.Host]!,
                UserName = configuration[RabbitMqNamings.Username]!,
                Password = configuration[RabbitMqNamings.Password]!,
                Port = Convert.ToInt32(configuration[RabbitMqNamings.Port]!)
            };
        }
        public async Task ConsumeAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }
            var channel = await _connection.CreateChannelAsync();
            string routingKey = "product.delete";
            string queueName = "orders.product.delete.queue";
            string exchangeName = _configuration[RabbitMqNamings.ProductsExchange]!;
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
                routingKey: routingKey
            );
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    string message = System.Text.Encoding.UTF8.GetString(args.Body.ToArray());
                    ProductDeleteMessage productDeleteMessage = JsonSerializer.Deserialize<ProductDeleteMessage>(message)!;
                    _logger.LogInformation("Received delete product message: {ProductID}", productDeleteMessage.ProductID);
                    string? cacheKey = "product_" + productDeleteMessage.ProductID.ToString();
                    await _distributedCache.RemoveAsync(cacheKey);
                    _logger.LogInformation("Removed product with ID {ProductID} from cache", productDeleteMessage.ProductID);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
                }
            };
            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );
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
