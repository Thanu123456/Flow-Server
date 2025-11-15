using Flow_Api.Data.Contexts;
using Flow_Api.Dtos.Brands.Request;
using Flow_Api.Dtos.Brands.Response;
using Flow_Api.Models.Entities.Tenant.Management;
using Flow_Api.Services.Interfaces.Management;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace Flow_Api.Services.Implementations.Management
{
    public class BrandService : IBrandService
    {

        private readonly TenantDbContext _context;
        public BrandService(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BrandDto>> GetBrandsAsync(string? search = null, string? status = null, int page = 1, int limit = 10)
        {
            var query = _context.Brands.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b => EF.Functions.ILike(b.Name, $"%{search}%"));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(b => b.Status.ToLower() == status.ToLower());

            var brands = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return brands.Select(b => MapToDto(b));
        }

        public async Task<BrandDto?> GetBrandByIdAsync(Guid id)
        {
            var brand = await _context.Brands.FindAsync(id);
            return brand == null ? null : MapToDto(brand);
        }

        public async Task<BrandDto> CreateBrandAsync(CreateBrandRequestDto dto)
        {
            var exists = await _context.Brands.AnyAsync(b => b.Name.ToLower() == dto.Name.ToLower());
            if (exists)
                throw new Exception("Brand name already exists.");

            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                ImageBase64 = dto.ImageBase64?.Contains(",") == true 
                    ? dto.ImageBase64.Split(",")[1] 
                    : dto.ImageBase64,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return MapToDto(brand);
        }

        public async Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandRequestDto dto)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                throw new Exception("Brand not found.");

            if (brand.Name != dto.Name)
            {
                var exists = await _context.Brands.AnyAsync(b => b.Name.ToLower() == dto.Name.ToLower());
                if (exists)
                    throw new Exception("Another brand with this name already exists.");
            }

            brand.Name = dto.Name;
            brand.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.ImageBase64))
                brand.ImageBase64 = dto.ImageBase64;
            brand.Status = dto.Status;
            brand.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(brand);
        }

        private static BrandDto MapToDto(Brand? b)
        {
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            return new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageBase64 = b.ImageBase64,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            };
        }

        public Task<int> GetBrandProductCountAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ExportBrandsToPDFAsync()
        {
            var brands = await _context.Brands.OrderBy(b => b.Name).ToListAsync();

            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document();
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                document.Open();

                // Title
                document.Add(new iTextSharp.text.Paragraph("Brand List"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                // Table with columns
                var table = new iTextSharp.text.pdf.PdfPTable(4);
                table.AddCell("Name");
                table.AddCell("Description");
                table.AddCell("Status");
                table.AddCell("Created At");

                foreach (var b in brands)
                {
                    table.AddCell(b.Name);
                    table.AddCell(b.Description ?? "-");
                    table.AddCell(b.Status);
                    table.AddCell(b.CreatedAt.ToString("yyyy-MM-dd"));
                }

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
        }

        public async Task<byte[]> ExportBrandsToExcelAsync()
        {
            var brands = await _context.Brands.OrderBy(b => b.Name).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Brands");

                // Header row
                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Description";
                worksheet.Cell(1, 3).Value = "Status";
                worksheet.Cell(1, 4).Value = "Created At";

                // Style header row
                var headerRange = worksheet.Range(1, 1, 1, 4);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data rows
                int row = 2;
                foreach (var b in brands)
                {
                    worksheet.Cell(row, 1).Value = b.Name;
                    worksheet.Cell(row, 2).Value = b.Description ?? "-";
                    worksheet.Cell(row, 3).Value = b.Status;
                    worksheet.Cell(row, 4).Value = b.CreatedAt.ToString("yyyy-MM-dd");
                    row++;
                }

                // Auto-fit columns for better readability
                worksheet.Columns().AdjustToContents();

                // Save to memory stream
                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
