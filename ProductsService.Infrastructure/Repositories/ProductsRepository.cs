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
            // We create a stub entity with the ID, so EF can delete it without querying it first
            var stubProduct = new Product { ProductID = productID };
            _dbContext.Products.Remove(stubProduct);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(conditionExpression) ?? null;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return await _dbContext.Products.AsNoTracking().Where(conditionExpression).ToListAsync();
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }
    }
}
