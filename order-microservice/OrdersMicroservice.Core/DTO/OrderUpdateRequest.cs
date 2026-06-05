namespace OrdersMicroservice.Core.DTO
{
    public class OrderUpdateRequest
    {
        public Guid OrderID { get; set; }
        public Guid UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemUpdateRequest> OrderItems { get; set; } = [];
    }
}
