using Flow_Api.Dtos.Categories.Request;
using Flow_Api.Services.Interfaces.Management;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flow_Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("all")]
        //role based access and authorization aprt add krnna
        public async Task<IActionResult> GetAll() 
        {
            return Ok(await _categoryService.GetCategoriesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _categoryService.GetCategoryByIDAsync(id));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequestDto createDTO)
        {
            return Ok(await _categoryService.CreateCategoryAsync(createDTO));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Updatecategory(Guid id, UpdateCategoryRequestDto updateDTO)
        {
            updateDTO.Id = id;
            return Ok(await _categoryService.UpdateCategoryAsync(updateDTO));
        }

        public async Task<IActionResult> DeleteCategory(Guid id) 
        {
            return Ok(await _categoryService.DeleteCategoryByIDAsync(id));
        }

    }
}
