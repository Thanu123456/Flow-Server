using Flow_Api.Dtos.Brands.Request;
using Flow_Api.Services.Interfaces.Management;
using Microsoft.AspNetCore.Mvc;


namespace Flow_Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandsController(IBrandService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands([FromQuery] string? search, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var brands = await _service.GetBrandsAsync(search, status, page, limit);
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var brand = await _service.GetBrandByIdAsync(id);
            if (brand == null) return NotFound(new {message = "Brand Not Found!"});
            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandRequestDto dto)
        {
            try
            {
                var brand = await _service.CreateBrandAsync(dto);
                return CreatedAtAction(nameof(GetBrandById), new { id = brand.Id }, brand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(Guid id, UpdateBrandRequestDto dto)
        {
            try
            {
                var brand = await _service.UpdateBrandAsync(id, dto);
                return Ok(brand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        } 

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportBrandsToPdf()
        {
            var pdfBytes = await _service.ExportBrandsToPDFAsync();
            return File(pdfBytes, "application/pdf", "Brands.pdf");
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportBrandsToExcel()
        {
            var excelBytes = await _service.ExportBrandsToExcelAsync();
            return File(excelBytes, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            "Brands.xlsx");
        }   
    }
}
