using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.Core.RepositoryContracts;
using ProductsService.Infrastructure.Context;
using ProductsService.Infrastructure.Repositories;

namespace ProductsService.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// A method for adding infrastructure services to the DI container. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Add the DbContext and Repository to the DI container
            services.AddDbContext<MySqlDbContext>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("MySqlConnection")!);
            });

            services.AddTransient<IProductsRepository, ProductsRepository>();

            return services;
        } 
    }
}
