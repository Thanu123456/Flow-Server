using Flow_Api.Dtos.Auth.Response;
using Flow_Api.Dtos.SuperAdmin.Response;
using Flow_Api.Models.Entities.Master;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TenantEntity = Flow_Api.Models.Entities.Master.Tenant;
using AutoMapper;

namespace Flow_Api.Mappings
{
    public class MasterMappingProfile : Profile
    {
        public MasterMappingProfile()
        {
            // User mappings
            CreateMap<User, UserInfoResponseDto>()
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src =>
                    src.Tenant != null ? src.Tenant.ShopName : null))
                .ForMember(dest => dest.SchemaName, opt => opt.MapFrom(src =>
                    src.Tenant != null ? src.Tenant.SchemaName : null));

            // Tenant mappings
            CreateMap<TenantEntity, TenantDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Owner.Email))
                .ForMember(dest => dest.RegisteredDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastActive, opt => opt.MapFrom(src => src.LastActiveAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.RegistrationStatus));

            CreateMap<TenantEntity, PendingRegistrationDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Owner.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Owner.PhoneNumber))
                .ForMember(dest => dest.RegisteredDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DaysPending, opt => opt.MapFrom(src => (DateTime.UtcNow - src.CreatedAt).Days))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress ?? "N/A"));

            // AuditLog mappings
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.FullName : "System"))
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src =>
                    src.Tenant != null ? src.Tenant.ShopName : null));
        }
    }
}
