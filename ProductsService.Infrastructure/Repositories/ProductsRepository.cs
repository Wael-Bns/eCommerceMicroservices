using System.Linq.Expressions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Entities;
using ProductsService.Core.RepositoryContracts;
using ProductsService.Infrastructure.Context;

namespace ProductsService.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly MySqlDbContext _dbContext;
        public ProductsRepository(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> AddProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            Product? foundProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productID) ?? null;
            if (foundProduct == null)
            {
                return false;
            }
            _dbContext.Products.Remove(foundProduct);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(conditionExpression) ?? null;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return await _dbContext.Products.Where(conditionExpression).ToListAsync();
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            Product? foundProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == product.ProductID) ?? null;
            if (foundProduct == null)
            {
                return null;
            }
            foundProduct.ProductName = product.ProductName;
            foundProduct.Category = product.Category;
            foundProduct.UnitPrice = product.UnitPrice;
            foundProduct.QuantityStock = product.QuantityStock;
            await _dbContext.SaveChangesAsync();
            return foundProduct;
        }
    }
}
