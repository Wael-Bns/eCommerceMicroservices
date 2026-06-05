namespace OrdersMicroservice.Core.RabbitMQ.ConsumerContracts
{
    public interface IRabbitMQDeleteProductConsumer
    {
        Task ConsumeAsync();
        ValueTask DisposeAsync();
    }
}