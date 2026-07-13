using AutoMapper;
using Backend.Auth;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Models;

namespace Backend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"))
            .ForMember(dest => dest.CanAdd, opt => opt.MapFrom(src => src.Role.HasLegacyPermission(Permissions.CanAdd)))
            .ForMember(dest => dest.CanEdit, opt => opt.MapFrom(src => src.Role.HasLegacyPermission(Permissions.CanEdit)))
            .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.Role.HasLegacyPermission(Permissions.CanDelete)))
            .ForMember(dest => dest.CanAccessDashboard, opt => opt.MapFrom(src => src.Role.HasLegacyPermission(Permissions.CanAccessDashboard)));
        CreateMap<User, UserResponse>().ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"));
        CreateMap<UserCreateRequest, User>();
        CreateMap<UserUpdateRequest, User>();

        // Role Mappings
        CreateMap<Role, RoleResponse>();
        CreateMap<RoleCreateRequest, Role>();
        CreateMap<RoleUpdateRequest, Role>();
        CreateMap<Permission, PermissionDto>();
    }
}