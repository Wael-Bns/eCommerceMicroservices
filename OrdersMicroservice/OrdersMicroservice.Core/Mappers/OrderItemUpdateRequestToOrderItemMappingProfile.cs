using AutoMapper;
using OrdersMicroservice.Core.DTO;
using OrdersMicroservice.Core.Entities;

namespace OrdersMicroservice.Core.Mappers
{
    public class OrderItemUpdateRequestToOrderItemMappingProfile : Profile
    {
        public OrderItemUpdateRequestToOrderItemMappingProfile()
        {
            CreateMap<OrderItemUpdateRequest, OrderItem>()
                .ForMember(dest => dest._id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());
        }
    }
}