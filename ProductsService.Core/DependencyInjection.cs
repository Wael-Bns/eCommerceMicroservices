using Microsoft.Extensions.DependencyInjection;
using ProductsService.Core.Mappers;
using ProductsService.Core.ServiceContracts;
using ProductsService.Core.Services;

namespace ProductsService.Core
{
    public static class DependencyInjection
    {
        /// <summary>
        /// A method for adding core services to the DI container. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            //Add AutoMapper profiles to the DI container
            services.AddScoped<IProductService, ProductService>();
            services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);

            return services;
        }
    }
}
