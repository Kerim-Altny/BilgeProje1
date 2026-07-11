using AutoMapper;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"))
            .ForMember(dest => dest.CanAdd, opt => opt.MapFrom(src => src.Role != null && src.Role.CanAdd))
            .ForMember(dest => dest.CanEdit, opt => opt.MapFrom(src => src.Role != null && src.Role.CanEdit))
            .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.Role != null && src.Role.CanDelete))
            .ForMember(dest => dest.CanAccessDashboard, opt => opt.MapFrom(src => src.Role != null && src.Role.CanAccessDashboard));
        CreateMap<User, UserResponse>().ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"));
        CreateMap<UserCreateRequest, User>();
        CreateMap<UserUpdateRequest, User>();

        // Role Mappings
        CreateMap<Role, RoleResponse>();
        CreateMap<RoleCreateRequest, Role>();
        CreateMap<RoleUpdateRequest, Role>();
    }
}