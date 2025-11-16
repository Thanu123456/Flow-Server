using Flow_Api.Dtos.Brands.Request;
using Flow_Api.Dtos.Brands.Response;

namespace Flow_Api.Services.Interfaces.Management
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetBrandsAsync(string? search, string? status, int page = 1, int limit = 10);

        Task<BrandDto?> GetBrandByIdAsync(Guid id);

        Task<BrandDto> CreateBrandAsync(CreateBrandRequestDto dto);

        Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandRequestDto dto);

        Task<int> GetBrandProductCountAsync(Guid id);

        Task<byte[]> ExportBrandsToPDFAsync();
        
        Task<byte[]> ExportBrandsToExcelAsync();
    }
}
