using System.Linq.Expressions;
using ProductsService.Core.DTO;
using ProductsService.Core.Entities;

namespace ProductsService.Core.ServiceContracts
{
    /// <summary>
    /// Contract for the ProductService, to make CRUD Opertions on Products.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Returns the list of all products in the database
        /// </summary>
        /// <returns></returns>
        Task<List<ProductResponse>> GetProducts();
        /// <summary>
        /// Returns a product based on the provided condition
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        Task<ProductResponse?> GetProductByCondition(Expression<Func<Product,bool>> conditionExpression);
        /// <summary>
        /// Returns the list of products based on the provided condition
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns>the list of products found</returns>
        Task<List<ProductResponse>> GetProductsbyCondition(Expression<Func<Product, bool>> conditionExpression);
        /// <summary>
        /// Adds a new product to the database and returns the added product with its generated ProductID
        /// </summary>
        /// <param name="productAddRequest"></param>
        /// <returns>The added product if it is added successfully, null otherwise</returns>
        Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest);
        /// <summary>
        /// Updates an existing product in the database based on the provided ProductID and returns the updated product
        /// </summary>
        /// <param name="productUpdateRequest"></param>
        /// <returns>The updated product if update is successful, null otherwise</returns>
        Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest);
        /// <summary>
        /// Deletes a product from the database based on the provided productID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>true if the product is deleted and false otherwise</returns>
        Task<bool> DeleteProduct(Guid productID);
    }
}
