using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersMicroservice.Core.RabbitMQ;
using OrdersMicroservice.Core.RabbitMQ.ConsumerContracts;
using OrdersMicroservice.Core.RabbitMQ.Consumers;
using OrdersMicroservice.Core.ServiceContracts;
using OrdersMicroservice.Core.Services;
using OrdersMicroservice.Core.Validators;

namespace OrdersMicroservice.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddTransient<IValidationService, ValidationService>();
            // Add validators from the assembly containing OrderAddRequestValidator
            services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
            // Add automapper
            services.AddAutoMapper(cfg => {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });
            services.AddStackExchangeRedisCache(options =>
            {
                string redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
                string redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
                options.Configuration = $"{redisHost}:{redisPort}"; 
            });

            services.AddTransient<IRabbitMQProductNameUpdateConsumer, RabbitMQProductNameUpdateConsumer>();
            services.AddTransient<IRabbitMQDeleteProductConsumer, RabbitMQDeleteProductConsumer>();
            services.AddHostedService<ProductHostedService>();
            return services;
        }
    }
}
