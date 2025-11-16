using Flow_Api.Dtos.Categories.Request;
using Flow_Api.Dtos.Categories.Response;
using Flow_Api.Models.Entities.Tenant.Management;

namespace Flow_Api.Services.Interfaces.Management
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto categoryDTO);
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequestDto updaetCategoryDTO);
        Task<CategoryDto> GetCategoryByIDAsync(Guid categoryID);
        Task<CategoryDto> DeleteCategoryByIDAsync(Guid categoryId);
    }
}
