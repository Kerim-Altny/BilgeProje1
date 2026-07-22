using AutoMapper;
using Backend.Auth;
using Backend.DTOs;
using Backend.DTOs.Links;

using Backend.Models;

namespace Backend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Role != null ? src.Role.RolePermissions.Select(rp => rp.Permission!.Name).ToList() : new List<string>()));
        CreateMap<User, UserResponse>().ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Bilinmeyen"));
        CreateMap<UserCreateRequest, User>();
        CreateMap<UserUpdateRequest, User>();


        CreateMap<Role, RoleResponse>().ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission!.Name).ToList()));
        CreateMap<RoleCreateRequest, Role>();
        CreateMap<RoleUpdateRequest, Role>();
        CreateMap<Permission, PermissionDto>();
        CreateMap<ShortLink, ShortLinkResponse>();
        CreateMap<ShortLink, AdminLinkResponse>()
     .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Username : "Bilinmeyen"))
     .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CreatedByUserId));


    }
}