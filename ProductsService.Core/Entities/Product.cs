namespace ProductsService.Core.Entities;
public class Product
{
    public Guid ProductID { get; set; }
    public string ProductName { get; set; }
    public string Category { get; set; }
    public double? UnitPrice { get; set; }
    public int? QuantityStock { get; set; }
}
