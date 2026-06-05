using System.Linq.Expressions;
using ProductsService.Core.Entities;

namespace ProductsService.Core.RepositoryContracts
{
    /// <summary>
    /// Repository is for managing data acces for the product table
    /// </summary>
    public interface IProductsRepository
    {
        /// <summary>
        /// Get all products in the database
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetProducts();
        /// <summary>
        /// Get a list of products by condition
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);
        /// <summary>
        /// get a product by a condition
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);
        /// <summary>
        /// Add a product in the database
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<Product?> AddProduct(Product product);
        /// <summary>
        /// Update the product in the database
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<Product?> UpdateProduct(Product product);
        /// <summary>
        /// delete a product given its Id
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>true if the product was found and deleted, false otherwise</returns>
        Task<bool> DeleteProduct(Guid productID);
    }
}
