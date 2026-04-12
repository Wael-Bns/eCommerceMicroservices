using AutoMapper;
using OrdersMicroservice.Core.DTO;
using OrdersMicroservice.Core.Entities;

namespace OrdersMicroservice.Core.Mappers
{
    public class ProductDTOToOrderItemResponseMappingProfile : Profile
    {
        public ProductDTOToOrderItemResponseMappingProfile()
        {
            CreateMap<ProductDTO, OrderItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
        }
    }
}