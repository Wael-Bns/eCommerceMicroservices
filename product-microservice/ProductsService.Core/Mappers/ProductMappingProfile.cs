using AutoMapper;
using ProductsService.Core.DTO;
using ProductsService.Core.Entities;
using SystemEnum = System.Enum; // Aliased to prevent conflicts

namespace ProductsService.Core.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => 
                    SystemEnum.Parse<CategoryOptions>(src.Category, true))) 
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock));
        }
    }
}
