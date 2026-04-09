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
            string temproaryConnectionString = configuration.GetConnectionString("MySqlConnection")!;
            
            string connectionString = temproaryConnectionString
                .Replace("$MYSQL_HOST",Environment.GetEnvironmentVariable("MYSQL_HOST")!)
                .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD")!)
                .Replace("$MYSQL_USER", Environment.GetEnvironmentVariable("MYSQL_USER")!)
                .Replace("$MYSQL_DATABASE", Environment.GetEnvironmentVariable("MYSQL_DATABASE")!)
                .Replace("$MYSQL_PORT", Environment.GetEnvironmentVariable("MYSQL_PORT")!);

            services.AddDbContext<MySqlDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });

            services.AddTransient<IProductsRepository, ProductsRepository>();

            return services;
        } 
    }
}
