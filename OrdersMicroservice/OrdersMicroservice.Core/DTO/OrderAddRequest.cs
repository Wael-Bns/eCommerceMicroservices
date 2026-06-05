namespace OrdersMicroservice.Core.DTO
{
    public class OrderAddRequest
    {
        public Guid UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemAddRequest> OrderItems { get; set; } = [];
    }
}
