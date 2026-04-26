namespace ProductsService.Core.DTO
{
    public class ProductResponse
    {
        public Guid ProductID { get; set; }
        public string ProductName { get; set; }
        public CategoryOptions Category { get; set; }
        public double? UnitPrice { get; set; }
        public int? QuantityInStock { get; set; }
    }
}
