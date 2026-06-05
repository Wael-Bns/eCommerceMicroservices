namespace OrdersMicroservice.Core.DTO
{
    public class OrderItemAddRequest
    {
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
