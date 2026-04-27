using Microsoft.Extensions.Hosting;
using OrdersMicroservice.Core.RabbitMQ.ConsumerContracts;

namespace OrdersMicroservice.Core.RabbitMQ
{
    public class ProductHostedService : IHostedService
    {
        private readonly IRabbitMQProductNameUpdateConsumer _rabbitMQProductNameUpdateConsumer;
        private readonly IRabbitMQDeleteProductConsumer _rabbitMQDeleteProductConsumer;
        public ProductHostedService(IRabbitMQProductNameUpdateConsumer productNameUpdateConsumer, IRabbitMQDeleteProductConsumer deleteConsumer)
        {
            _rabbitMQProductNameUpdateConsumer = productNameUpdateConsumer;
            _rabbitMQDeleteProductConsumer = deleteConsumer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _rabbitMQProductNameUpdateConsumer.ConsumeAsync();
            await _rabbitMQDeleteProductConsumer.ConsumeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _rabbitMQProductNameUpdateConsumer.DisposeAsync();
            await _rabbitMQDeleteProductConsumer.DisposeAsync();
        }
    }
}
