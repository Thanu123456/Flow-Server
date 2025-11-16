using Flow_Api.Data.Contexts;
using Flow_Api.Dtos.Categories.Request;
using Flow_Api.Dtos.Categories.Response;
using Flow_Api.Models.Entities.Tenant.Management;
using Flow_Api.Services.Interfaces.Management;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Services.Implementations.Management
{
    public class CategoryService : ICategoryService
    {
        private readonly TenantDbContext _TenantDBContext;
        public CategoryService(TenantDbContext tenantDBContext) 
        {
            _TenantDBContext = tenantDBContext;
        }

        //auto generate category code:
        private async Task<string> GenerateCategoryCodeAsync()
        {
            var lastCode = await _TenantDBContext.Categories
                                .OrderByDescending(c => c.CreatedAt)
                                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastCode != null)
            {
                var numeric = lastCode.Code.Replace("CAT-", "");
                int.TryParse(numeric, out nextNumber);
                nextNumber++;
            }
            return $"CAT-{nextNumber:D3}";
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto categoryDTO)
        {
            //meka remove krnna data model eke name ekatai code aktai required daala
            if (string.IsNullOrWhiteSpace(categoryDTO.Name)) throw new Exception("Name is required");

            bool nameExist = await _TenantDBContext.Categories.AnyAsync(x => x.Name == categoryDTO.Name && !x.IsDeleted);

            if (nameExist) throw new Exception("Category name already exists");

            string code = await GenerateCategoryCodeAsync();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryDTO.Name,
                Code = code,
                Description = categoryDTO.Description,
                Status = categoryDTO.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _TenantDBContext.Categories.Add(category);
            await _TenantDBContext.SaveChangesAsync();

            //audit log ekat log krnna methanin save krpu welawela me comment ekt yatin

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Code = category.Code,
                Status = category.Status,
            };

        }

        public async Task<CategoryDto> DeleteCategoryByIDAsync(Guid categoryId)
        {
            var category = await _TenantDBContext.Categories
                                .Where(c => c.Id == categoryId && !c.IsDeleted)
                                .FirstOrDefaultAsync();

            if (category == null) 
            {
                throw new Exception("category not found");
            }

            //sub count thiywada balnna eka balala soft delet rknna logic eka hdnna subactegory haduwain passe
            int subCount = 0;

            if (subCount > 0)
            {
                category.IsDeleted = true;
                category.UpdatedAt = DateTime.UtcNow;
            }
            else 
            {
                _TenantDBContext.Categories.Remove(category);
            }

            await _TenantDBContext.SaveChangesAsync();
            //audit log eka update krnna methanin passe save kalahama

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Code = category.Code,
                Status = category.Status,
            };
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _TenantDBContext.Categories
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();
        }

        public async Task<CategoryDto> GetCategoryByIDAsync(Guid categoryID)
        {
            var category = await _TenantDBContext.Categories
                                 .Where(x => x.Id == categoryID && !x.IsDeleted).FirstOrDefaultAsync();
            if (category == null) 
            {
                throw new Exception("Category was not found");
            }
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status,
                Code = category.Code,
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequestDto updaetCategoryDTO)
        {
            var category = await _TenantDBContext.Categories
                .Where(c => c.Id == updaetCategoryDTO.Id && !c.IsDeleted)
                .FirstOrDefaultAsync() ?? throw new Exception("Category not found");

            bool uniqueCheck = await _TenantDBContext.Categories
                                    .AnyAsync(c => c.Name == updaetCategoryDTO.Name && !c.IsDeleted);

            if (uniqueCheck) throw new Exception("Name already exists on same or diffrent category");

            category.Name = updaetCategoryDTO.Name;
            category.Description = updaetCategoryDTO.Description;
            category.Status = updaetCategoryDTO.Status;
            category.UpdatedAt = DateTime.UtcNow;

            await _TenantDBContext.SaveChangesAsync();

            //mthanin save kalain passe audit log eka ghnna

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Code = category.Code,
                Status = category.Status
            };

        }
    }
}
