using System.Linq.Expressions;
using AutoMapper;
using MongoDB.Driver;
using OrdersMicroservice.Core.DTO;
using OrdersMicroservice.Core.Entities;
using OrdersMicroservice.Core.HttpClients;
using OrdersMicroservice.Core.RepositoryContracts;
using OrdersMicroservice.Core.ServiceContracts;

namespace OrdersMicroservice.Core.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        private readonly UsersMicroserviceClient _usersMicroserviceClient;
        private readonly ProductsMicroserviceClient _productsMicroserviceClient;
        public OrdersService(IOrdersRepository ordersRepository, IMapper mapper, IValidationService validationService, UsersMicroserviceClient usersMicroserviceClient, ProductsMicroserviceClient productsMicroserviceClient)
        {
            _ordersRepository = ordersRepository;
            _mapper = mapper;
            _validationService = validationService;
            _usersMicroserviceClient = usersMicroserviceClient;
            _productsMicroserviceClient = productsMicroserviceClient;
        }
        public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
        {
            if(orderAddRequest == null)
                            {
                throw new ArgumentNullException(nameof(orderAddRequest));
            }
            // Validate the orderAddRequest
            await _validationService.ValidateAsync(orderAddRequest);

            // Validate if the user exists by calling the users microservice
            UserDTO? foundUser = await _usersMicroserviceClient.GetUserByUserID(orderAddRequest.UserID);
            if(foundUser == null)
            {
                throw new ArgumentException("User Id provided in the order request does not exist", nameof(orderAddRequest.UserID));
            }

            // Validate if the products exist by calling the products microservice
            foreach (var item in orderAddRequest.OrderItems)
            {
                ProductDTO? foundProduct = await _productsMicroserviceClient.GetProductByProductID(item.ProductID);
                if(foundProduct == null)
                {
                    throw new ArgumentException($"Product Id {item.ProductID} provided in the order request does not exist", nameof(item.ProductID));
                }
            }

            // Add the order if no exception has occurred during validation
            Order? order = await _ordersRepository.AddOrder(_mapper.Map<Order>(orderAddRequest));
            return _mapper.Map<OrderResponse?>(order);
        }

        public async Task<bool> DeleteOrder(Guid orderId)
        {
            if(orderId == Guid.Empty)
            {
                throw new ArgumentException("OrderId cannot be empty", nameof(orderId));
            }
            bool isDeleted = await _ordersRepository.DeleteOrder(orderId);
            return isDeleted;
        }

        public async Task<OrderResponse?> GetOrderByCondition(Expression<Func<Order, bool>> filter)
        {
            // Validate the filter
            ArgumentNullException.ThrowIfNull(filter, nameof(filter));
            // Get the order based on the filter
            Order? order = await _ordersRepository.GetOrderByCondition(filter);
            OrderResponse? orderResponse = _mapper.Map<OrderResponse?>(order);
            if (orderResponse != null)
            {
                foreach (OrderItemResponse orderItem in orderResponse.OrderItems)
                {
                    ProductDTO? productDTO = await _productsMicroserviceClient.GetProductByProductID(orderItem.ProductID);
                    if (productDTO == null)
                        continue;
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);
                }
            }

            return orderResponse;   
        }

        public async Task<List<OrderResponse>> GetOrders()
        {
            List<Order> orders = (await _ordersRepository.GetOrders()).ToList();
            List<OrderResponse> orderResponses = _mapper.Map<List<OrderResponse>>(orders);
            foreach(OrderResponse orderResponse in orderResponses)
            {
                if (orderResponse == null)
                    continue;
                foreach(OrderItemResponse orderItem in orderResponse.OrderItems)
                {
                    ProductDTO? productDTO = await _productsMicroserviceClient.GetProductByProductID(orderItem.ProductID);
                    if (productDTO == null)
                        continue;
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);
                }
            }
            return orderResponses.ToList();
        }

        public async Task<List<OrderResponse>> GetOrdersByCondition(Expression<Func<Order, bool>> filter)
        {
            // Validate the filter
            ArgumentNullException.ThrowIfNull(filter,nameof(filter));
            // Get the orders based on the filter
            List<Order> orders = (await _ordersRepository.GetOrdersByCondition(filter)).ToList()!;
            // Map the orders to order responses
            List<OrderResponse> orderResponses = _mapper.Map<List<OrderResponse>>(orders);
            foreach (OrderResponse orderResponse in orderResponses)
            {
                if (orderResponse == null)
                    continue;
                foreach (OrderItemResponse orderItem in orderResponse.OrderItems)
                {
                    ProductDTO? productDTO = await _productsMicroserviceClient.GetProductByProductID(orderItem.ProductID);
                    if (productDTO == null)
                        continue;
                    _mapper.Map<ProductDTO, OrderItemResponse>(productDTO, orderItem);
                }
            }
            return orderResponses.ToList();
        }

        public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(orderUpdateRequest));
            }
            // Validate the orderAddRequest
            await _validationService.ValidateAsync(orderUpdateRequest);
            // Validate if the user exists by calling the users microservice
            UserDTO? foundUser = await _usersMicroserviceClient.GetUserByUserID(orderUpdateRequest.UserID);
            if (foundUser == null)
            {
                throw new ArgumentException("User Id provided in the order request does not exist", nameof(orderUpdateRequest.UserID));
            }

            // Validate if the products exist by calling the products microservice
            foreach (var item in orderUpdateRequest.OrderItems)
            {
                ProductDTO? foundProduct = await _productsMicroserviceClient.GetProductByProductID(item.ProductID);
                if (foundProduct == null)
                {
                    throw new ArgumentException($"Product Id {item.ProductID} provided in the order request does not exist", nameof(item.ProductID));
                }
            }

            // Add the order if no exception has occurred during validation
            Order? order = await _ordersRepository.UpdateOrder(_mapper.Map<Order>(orderUpdateRequest));
            return _mapper.Map<OrderResponse?>(order);
        }
    }
}
