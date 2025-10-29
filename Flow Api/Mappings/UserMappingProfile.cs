using AutoMapper;
using Flow_Api.Dtos.User.Response;
using Flow_Api.Models.Entities.Master;

namespace Flow_Api.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src =>
                    src.Tenant != null ? src.Tenant.ShopName : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.RoleName, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseName, opt => opt.Ignore());
        }
    }
}
