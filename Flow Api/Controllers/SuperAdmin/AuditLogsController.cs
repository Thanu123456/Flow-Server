using AutoMapper;
using Flow_Api.Attributes;
using Flow_Api.Data.UnitOfWork;
using Flow_Api.Dtos.Common;
using Flow_Api.Dtos.SuperAdmin.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Flow_Api.Controllers.SuperAdmin
{
    [Authorize]
    [RequireSuperAdmin]
    public class AuditLogsController : BaseApiController
    {
        private readonly IMasterUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuditLogsController(IMasterUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetAuditLogs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var logs = await _unitOfWork.AuditLogs.GetAllAsync();

            var pagedLogs = logs
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var logDtos = _mapper.Map<IEnumerable<AuditLogDto>>(pagedLogs);

            return Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
                logDtos,
                "Audit logs retrieved"
            ));
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetTenantAuditLogs(Guid tenantId)
        {
            var logs = await _unitOfWork.AuditLogs.GetByTenantIdAsync(tenantId);
            var logDtos = _mapper.Map<IEnumerable<AuditLogDto>>(logs);

            return Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
                logDtos,
                "Tenant audit logs retrieved"
            ));
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetUserAuditLogs(Guid userId)
        {
            var logs = await _unitOfWork.AuditLogs.GetByUserIdAsync(userId);
            var logDtos = _mapper.Map<IEnumerable<AuditLogDto>>(logs);

            return Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
                logDtos,
                "User audit logs retrieved"
            ));
        }
    }
}
