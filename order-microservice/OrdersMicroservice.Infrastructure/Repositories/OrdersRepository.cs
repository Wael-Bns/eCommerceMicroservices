using System.Linq.Expressions;
using MongoDB.Driver;
using OrdersMicroservice.Core.Entities;
using OrdersMicroservice.Core.RepositoryContracts;

namespace OrdersMicroservice.Infrastructure.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly string collectionName = "orders";
        public OrdersRepository(IMongoDatabase database)
        {
            _orders = database.GetCollection<Order>(collectionName);
        }
        public async Task<Order> AddOrder(Order order)
        {
            order.OrderID = Guid.NewGuid();
            order._id = order.OrderID;

            foreach(OrderItem orderItem in order.OrderItems)
            {
                orderItem._id = Guid.NewGuid();
            }

            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<bool> DeleteOrder(Guid orderId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(order => order.OrderID, orderId);

            DeleteResult result = await _orders.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }

        public async Task<Order?> GetOrderByCondition(Expression<Func<Order, bool>> condition)
        {
            Order? order = (await _orders.FindAsync(condition)).FirstOrDefault();
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return (await _orders.FindAsync(order => true)).ToList();
        }

        public async Task<IEnumerable<Order?>> GetOrdersByCondition(Expression<Func<Order, bool>> condition)
        {
            return (await _orders.FindAsync(condition)).ToList();
        }

        public async Task<Order?> UpdateOrder(Order order)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(existingOrder => existingOrder.OrderID, order.OrderID);
            
            Order? existingOrder = (await _orders.FindAsync(filter)).FirstOrDefault();
            if (existingOrder == null)
            {
                return null;
            }

            // Preserve the immutable MongoDB _id 
            order._id = existingOrder._id;

            ReplaceOneResult result = await _orders.ReplaceOneAsync(filter, order);
            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return order;
            }
            else
            {
                return null;
            }
        }
    }
}
