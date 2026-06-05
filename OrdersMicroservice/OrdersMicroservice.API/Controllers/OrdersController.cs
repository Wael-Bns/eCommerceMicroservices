using Microsoft.AspNetCore.Mvc;
using OrdersMicroservice.Core.DTO;
using OrdersMicroservice.Core.ServiceContracts;

namespace OrdersMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _orderService;
        public OrdersController(IOrdersService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            return Ok(await _orderService.GetOrders());
        }
        [HttpGet("search/orderid/{orderId}")]
        public async Task<IActionResult> GetOrderByOrderId(Guid orderId)
        {
            OrderResponse? order = await _orderService.GetOrderByCondition(order => order.OrderID == orderId);
            if(order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpGet("search/productid/{productId}")]
        public async Task<IActionResult> GetOrdersByProductId(Guid productId)
        {
            List<OrderResponse> orders = await _orderService.GetOrdersByCondition(
                order => order.OrderItems.Any(o => o.ProductID == productId));

            return Ok(orders);

        }
        [HttpGet("search/orderdate/{orderDate}")]
        public async Task<IActionResult> GetOrdersByOrderDate(DateTime orderDate)
        {
            // Normalize the requested date down to exactly midnight (00:00)
            DateTime dateStart = orderDate.Date;
            
            // Calculate exactly midnight of the NEXT day
            DateTime dateEnd = dateStart.AddDays(1);

            List<OrderResponse> orders = await _orderService.GetOrdersByCondition(
                order => order.OrderDate >= dateStart && order.OrderDate < dateEnd);

            return Ok(orders);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderAddRequest orderAddRequest)
        {
            OrderResponse? order = await _orderService.AddOrder(orderAddRequest);
            if (order != null)
                return CreatedAtAction(nameof(GetOrderByOrderId), new { orderId = order.OrderID }, order);

            return BadRequest();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
        {
            OrderResponse? order = await _orderService.UpdateOrder(orderUpdateRequest);
            if (order != null)
                return Ok(order);

            return BadRequest();
        }
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            bool isDeleted = await _orderService.DeleteOrder(orderId);
            if (isDeleted)
                return Ok("Deleted Successfully");
            return NotFound();
        }
    }
}
