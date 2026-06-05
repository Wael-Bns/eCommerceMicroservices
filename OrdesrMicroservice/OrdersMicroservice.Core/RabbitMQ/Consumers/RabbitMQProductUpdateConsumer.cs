using System.Text;
using System.Text.Json;
using DnsClient.Internal;
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
    public class RabbitMQProductUpdateConsumer : IRabbitMQProductNameUpdateConsumer
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private readonly ILogger<RabbitMQProductUpdateConsumer> _logger;
        private readonly IDistributedCache _distributedCache;
        public RabbitMQProductUpdateConsumer(IConfiguration configuration, ILogger<RabbitMQProductUpdateConsumer> logger, IDistributedCache distributedCache)
        {
            _configuration = configuration;
            _distributedCache = distributedCache;
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration[RabbitMqNamings.Host]!,
                UserName = configuration[RabbitMqNamings.Username]!,
                Password = configuration[RabbitMqNamings.Password]!,
                Port = Convert.ToInt32(configuration[RabbitMqNamings.Port]!)
            };
            _logger = logger;
        }
        private async Task HandleCacheUpdate(ProductUpdateMessage productUpdateMessage)
        {
            string? cacheKey = "product_" + productUpdateMessage.ProductID.ToString();
            var cacheOptions = new DistributedCacheEntryOptions()
                                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                                    .SetSlidingExpiration(TimeSpan.FromSeconds(100));
            if (_distributedCache.GetAsync(cacheKey) != null)
            {
                await _distributedCache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(productUpdateMessage), cacheOptions);
                _logger.LogInformation("Updated cache for ProductID: {ProductID}", productUpdateMessage.ProductID);
            }
        }
        public async Task ConsumeAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }
            var channel = await _connection.CreateChannelAsync();

            string routingKey = "product.update";
            string queueName = "orders.update.queue";

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

            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(args.Body.ToArray());
                    var productUpdate = JsonSerializer.Deserialize<ProductUpdateMessage>(message);
                    _logger.LogInformation("Received product update message: {Message}", message);
                    await HandleCacheUpdate(productUpdate!);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing product update message");
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
