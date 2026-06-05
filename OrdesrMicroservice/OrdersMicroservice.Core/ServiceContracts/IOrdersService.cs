using System.Linq.Expressions;
using MongoDB.Driver;
using OrdersMicroservice.Core.DTO;
using OrdersMicroservice.Core.Entities;

namespace OrdersMicroservice.Core.ServiceContracts
{
    public interface IOrdersService
    {
        /// <summary>
        /// Returns the list of all the orders in the data store.
        /// </summary>
        /// <returns></returns>
        Task<List<OrderResponse>> GetOrders();
        /// <summary>
        /// Returns a list of orders from the data store based on the specified filter condition.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<OrderResponse>> GetOrdersByCondition(Expression<Func<Order, bool>> filter);
        /// <summary>
        /// Returns a single order from the data store based on the specified filter condition.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<OrderResponse?> GetOrderByCondition(Expression<Func<Order, bool>> filter);
        /// <summary>
        /// Adds an order to the data store based on the details provided in the OrderAddRequest object.
        /// </summary>
        /// <param name="orderAddRequest"></param>
        /// <returns>Returns the newly generated order</returns>
        Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest);
        /// <summary>
        /// Updates an existing order in the data store based on the details provided in the OrderUpdateRequest object.
        /// </summary>
        /// <param name="orderUpdateRequest">Returns the updated order, and null if the update failed</param>
        /// <returns></returns>
        Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest);
        /// <summary>
        /// Deletes an order from the data store based on the provided order ID.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>Returns a boolean indicating if the order with the specified Id was deleted or not.</returns>
        Task<bool> DeleteOrder(Guid orderId);

    }
}
