using AutoMapper;
using HomeExchange.Data.Models;
using HomeExchange.DTOs;

namespace HomeExchange.Profiles
{
    public class HomeOwnerMappingProfile : Profile
    {
        public HomeOwnerMappingProfile() {
            CreateMap<Users, UserResponseDTO>();
            CreateMap<UserRequestDTO, Users>();
            
        }
    }
}
