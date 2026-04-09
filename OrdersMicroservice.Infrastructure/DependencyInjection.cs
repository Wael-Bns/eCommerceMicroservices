using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrdersMicroservice.Core.RepositoryContracts;
using OrdersMicroservice.Infrastructure.Repositories;

namespace OrdersMicroservice.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Build the connection string
            string connectionStringTemplate = configuration.GetConnectionString("MongoConnection")!;
            string connectionString = connectionStringTemplate
                .Replace("$MONGODB_HOST", Environment.GetEnvironmentVariable("MONGODB_HOST"))
                .Replace("$MONGODB_PORT", Environment.GetEnvironmentVariable("MONGODB_PORT"));
            // Register the MongoDB client
            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

            services.AddScoped<IMongoDatabase>(provider =>
            {
                IMongoClient mongoClient = provider.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_DATABASE"));
            });

            services.AddScoped<IOrdersRepository, OrdersRepository>();

            return services;
        }
    }
}
