namespace OrdersMicroservice.Core.RabbitMQ.ConsumerContracts
{
    public interface IRabbitMQProductNameUpdateConsumer
    {
        Task ConsumeAsync();
        ValueTask DisposeAsync();
    }
}