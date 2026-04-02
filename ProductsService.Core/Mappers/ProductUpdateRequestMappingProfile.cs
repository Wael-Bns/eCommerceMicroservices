using AutoMapper;
using ProductsService.Core.DTO;
using ProductsService.Core.Entities;

namespace ProductsService.Core.Mappers
{
    public class ProductUpdateRequestMappingProfile : Profile 
    {
        public ProductUpdateRequestMappingProfile()
        {
            CreateMap<ProductUpdateRequest, Product>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.QuantityStock, opt => opt.MapFrom(src => src.QuantityStock));
        }
    }
}
