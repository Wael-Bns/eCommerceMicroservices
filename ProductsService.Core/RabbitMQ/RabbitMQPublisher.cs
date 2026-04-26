using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ProductsService.Core.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher, IAsyncDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        public RabbitMQPublisher(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RABBITMQ_HOST"]!,
                UserName = configuration["RABBITMQ_USERNAME"]!,
                Password = configuration["RABBITMQ_PASSWORD"]!,
                Port = Convert.ToInt32(configuration["RABBITMQ_PORT"]!)
            };
        }
        public async Task PublishAsync<T>(string routekey, T message)
        {
            if(_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }
            await using var channel = await _connection.CreateChannelAsync();
            string messageJson = JsonSerializer.Serialize(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);
            string exchangeName = "products.exchange";
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
            );
            var properties = new BasicProperties
            {
                Persistent = true
            };
            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routekey,
                mandatory: false,
                basicProperties: properties,
                body: messageBytes);
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
