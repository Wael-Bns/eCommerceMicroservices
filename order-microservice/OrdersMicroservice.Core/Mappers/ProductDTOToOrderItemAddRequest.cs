using AutoMapper;
using OrdersMicroservice.Core.DTO;

namespace OrdersMicroservice.Core.Mappers
{
    public class ProductDTOToOrderItemAddRequest : Profile
    {
        public ProductDTOToOrderItemAddRequest()
        {
            CreateMap<ProductDTO, OrderItemAddRequest>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID));
        }
    }
}