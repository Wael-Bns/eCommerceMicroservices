namespace OrdersMicroservice.Core.RabbitMQ.Messages
{
    public record ProductUpdateMessage(
        Guid ProductID,
        string ProductName,
        string Category,
        double UnitPrice,
        int QuantityInStock
    );
}
