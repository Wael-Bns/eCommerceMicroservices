namespace ProductsService.Core.RabbitMQ.Messages
{
    public record ProductNameUpdateMessage(Guid ProductID, string? NewName); 
}
