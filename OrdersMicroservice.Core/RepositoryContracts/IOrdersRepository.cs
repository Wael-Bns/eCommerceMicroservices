using System.Linq.Expressions;
using OrdersMicroservice.Core.Entities;

namespace OrdersMicroservice.Core.RepositoryContracts
{
    public interface IOrdersRepository
    {
        /// <summary>
        /// Retrieves all orders from the data source.
        /// </summary>
        /// <returns>List of orders</returns>
        Task<IEnumerable<Order>> GetOrders();
        /// <summary>
        /// Retrieves orders that satisfy the specified condition.
        /// </summary>
        /// <param name="condition">An expression used to filter the orders.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of orders matching
        /// the condition.</returns>
        Task<IEnumerable<Order?>> GetOrdersByCondition(Expression<Func<Order, bool>> condition);
        /// <summary>
        /// Returns a single order that satisfies the specified condition.
        /// </summary>
        /// <param name="condition">An expression used to filter the orders.</param>
        /// <returns>The first order if it exists and null otherwise</returns>
        Task<Order?> GetOrderByCondition(Expression<Func<Order, bool>> condition);
        /// <summary>
        /// Adds a new order to the data source
        /// </summary>
        /// <param name="order">Order to add</param>
        /// <returns>The added order</returns>
        Task<Order> AddOrder(Order order);
        /// <summary>
        /// Updates a given order in the data source.
        /// </summary>
        /// <param name="order">the order to update</param>
        /// <returns>The order after the update</returns>
        Task<Order?> UpdateOrder(Order order);
        /// <summary>
        /// Delete an order from the data source based on the provided order ID.
        /// </summary>
        /// <param name="orderId">orderID of the order to delete</param>
        /// <returns>True if the order has been deleted, false otherwise</returns>
        Task<bool> DeleteOrder(Guid orderId);
    }
}
