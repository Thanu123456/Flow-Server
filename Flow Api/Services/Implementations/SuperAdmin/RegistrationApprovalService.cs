using Flow_Api.Data.UnitOfWork;
using Flow_Api.Dtos.SuperAdmin.Request;
using Flow_Api.Dtos.SuperAdmin.Response;
using Flow_Api.Exceptions;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Services.Interfaces.Notifications;
using Flow_Api.Services.Interfaces.SuperAdmin;
using TenantEntity = Flow_Api.Models.Entities.Master.Tenant;

namespace Flow_Api.Services.Implementations.SuperAdmin
{
    public class RegistrationApprovalService : IRegistrationApprovalService
    {
        private readonly IMasterUnitOfWork _unitOfWork;
        private readonly ITenantProvisioningService _provisioningService;
        private readonly IEmailService _emailService;
        private readonly IAuditLogService _auditLogService;

        public RegistrationApprovalService(
            IMasterUnitOfWork unitOfWork,
            ITenantProvisioningService provisioningService,
            IEmailService emailService,
            IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _provisioningService = provisioningService;
            _emailService = emailService;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<PendingRegistrationDto>> GetPendingRegistrationsAsync()
        {
            var pendingTenants = await _unitOfWork.Tenants.GetPendingRegistrationsAsync();

            return pendingTenants.Select(t => new PendingRegistrationDto
            {
                Id = t.Id,
                ShopName = t.ShopName,
                OwnerName = t.Owner.FullName,
                Email = t.Owner.Email,
                PhoneNumber = t.Owner.PhoneNumber,
                BusinessType = t.BusinessType,
                RegisteredDate = t.CreatedAt,
                DaysPending = (DateTime.UtcNow - t.CreatedAt).Days,
                IpAddress = t.IpAddress ?? "N/A"
            });
        }

        public async Task<bool> ApproveRegistrationAsync(ApproveRegistrationRequestDto request, Guid superAdminId)
        {
            var approvalTenant = await _unitOfWork.Tenants.GetByIdWithOwnerAsync(request.TenantId);

            if (approvalTenant == null)
                throw new NotFoundException("Tenant not found");

            if (approvalTenant.RegistrationStatus != RegistrationStatus.Pending)
                throw new BadRequestException("Registration is not in pending status");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Create schema for tenant
                var schemaName = await _provisioningService.CreateTenantSchemaAsync(
                    approvalTenant.Id,
                    approvalTenant.ShopName
                );

                // Update tenant status
                approvalTenant.RegistrationStatus = RegistrationStatus.Active;
                approvalTenant.SchemaName = schemaName;
                approvalTenant.ApprovedBy = superAdminId;
                approvalTenant.ApprovedAt = DateTime.UtcNow;

                // Activate owner account
                var owner = approvalTenant.Owner;
                owner.Status = UserStatus.Active;

                await _unitOfWork.SaveChangesAsync();

                // Run migrations for new schema
                await _provisioningService.RunMigrationsAsync(schemaName);

                // Seed default data
                await _provisioningService.SeedDefaultDataAsync(schemaName, approvalTenant.Id);

                // Log approval
                await _auditLogService.LogActionAsync(
                    AuditActionType.RegistrationApproved,
                    $"Registration approved for {approvalTenant.ShopName}",
                    superAdminId,
                    approvalTenant.Id
                );

                await _unitOfWork.CommitTransactionAsync();

                // Send approval email
                await _emailService.SendApprovalEmailAsync(
                    owner.Email,
                    owner.FullName,
                    approvalTenant.ShopName,
                    "https://yourapp.com/login"
                );

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> RejectRegistrationAsync(RejectRegistrationRequestDto request, Guid superAdminId)
        {
            var rejectionTenant = await _unitOfWork.Tenants.GetByIdWithOwnerAsync(request.TenantId);

            if (rejectionTenant == null)
                throw new NotFoundException("Tenant not found");

            if (rejectionTenant.RegistrationStatus != RegistrationStatus.Pending)
                throw new BadRequestException("Registration is not in pending status");

            // Update tenant status
            rejectionTenant.RegistrationStatus = RegistrationStatus.Rejected;
            rejectionTenant.RejectionReason = request.Reason;
            rejectionTenant.RejectedBy = superAdminId;
            rejectionTenant.RejectedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            // Log rejection
            await _auditLogService.LogActionAsync(
                AuditActionType.RegistrationRejected,
                $"Registration rejected for {rejectionTenant.ShopName}: {request.Reason}",
                superAdminId,
                rejectionTenant.Id
            );

            // Send rejection email
            var owner = rejectionTenant.Owner;
            await _emailService.SendRejectionEmailAsync(owner.Email, owner.FullName, request.Reason);

            return true;
        }
    }
}
