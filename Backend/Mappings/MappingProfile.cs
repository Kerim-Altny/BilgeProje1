using AutoMapper;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserProfileResponse>().ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"));
        CreateMap<User, UserResponse>().ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"));
        CreateMap<UserCreateRequest, User>();
        CreateMap<UserUpdateRequest, User>();
    }
}