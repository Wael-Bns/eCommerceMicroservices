namespace ProductsService.Core.RabbitMQ
{
    /// <summary>
    /// an interface for a RabbitMQ publisher, which is responsible for publishing messages to a RabbitMQ exchange.
    /// </summary>
    public interface IRabbitMQPublisher
    {
        /// <summary>
        /// a method to publish a message to a RabbitMQ exchange with a specific routing key.
        /// </summary>
        /// <typeparam name="T">the type of the message to be published</typeparam>
        /// <param name="routekey">the routing key to use for the message</param>
        /// <param name="message">the message to be published</param>
        Task PublishAsync<T>(string routekey, T message);
    }
}
