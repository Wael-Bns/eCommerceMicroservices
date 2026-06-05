using AutoMapper;
using ProductsService.Core.DTO;
using ProductsService.Core.Entities;

namespace ProductsService.Core.Mappers
{
    public class ProductAddRequestMappingProfile : Profile 
    {
        public ProductAddRequestMappingProfile()
        {
            CreateMap<ProductAddRequest, Product>()
                .ForMember(dest => dest.ProductID, opt => opt.Ignore())
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock));
        }
    }
}
