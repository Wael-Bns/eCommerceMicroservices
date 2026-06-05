namespace ProductsService.Core.DTO
{
    public class ProductAddRequest
    {
        public string ProductName { get; set; }
        public CategoryOptions Category { get; set; }
        public double? UnitPrice { get; set; }
        public int? QuantityInStock { get; set; }
    }
}
