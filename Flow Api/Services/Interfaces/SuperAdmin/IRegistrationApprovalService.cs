using Flow_Api.Dtos.SuperAdmin.Request;
using Flow_Api.Dtos.SuperAdmin.Response;

namespace Flow_Api.Services.Interfaces.SuperAdmin
{
    public interface IRegistrationApprovalService
    {
        Task<IEnumerable<PendingRegistrationDto>> GetPendingRegistrationsAsync();
        Task<bool> ApproveRegistrationAsync(ApproveRegistrationRequestDto request, Guid superAdminId);
        Task<bool> RejectRegistrationAsync(RejectRegistrationRequestDto request, Guid superAdminId);
    }
}
