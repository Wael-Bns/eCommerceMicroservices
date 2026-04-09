namespace OrdersMicroservice.Core.DTO
{
    public class OrderItemUpdateRequest
    {
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
