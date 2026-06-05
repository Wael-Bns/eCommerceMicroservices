using AutoMapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entities;

namespace eCommerce.Core.Mappers
{
    public class RegisterRequestMappingProfile : Profile
    {
        public RegisterRequestMappingProfile() 
        {
            // configure mapping from RegisterRequest to ApplicationUser
            CreateMap<RegisterRequest, ApplicationUser>()
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.PersonName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.UserID, opt => opt.Ignore());
        }
    }
}
