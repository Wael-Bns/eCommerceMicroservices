using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ProductsService.Core.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher, IAsyncDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private readonly ILogger<RabbitMQPublisher> _logger;
        public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RABBITMQ_HOST"]!,
                UserName = configuration["RABBITMQ_USERNAME"]!,
                Password = configuration["RABBITMQ_PASSWORD"]!,
                Port = Convert.ToInt32(configuration["RABBITMQ_PORT"]!)
            };
        }
        public async Task PublishAsync<T>(string routingKey, T message)
        {
            if(_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }
            await using var channel = await _connection.CreateChannelAsync();
            string messageJson = JsonSerializer.Serialize(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);
            string exchangeName = _configuration["RABBITMQ_EXCHANGE"]!;
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
            );
            var properties = new BasicProperties
            {
                Persistent = true,
            };
            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: messageBytes);
            _logger.LogInformation("message published to exchange {Exchange} with routing key {RoutingKey}", exchangeName, routingKey);
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
