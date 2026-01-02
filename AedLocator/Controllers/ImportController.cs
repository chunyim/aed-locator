using Microsoft.AspNetCore.Mvc;
using AedLocator.Models;
using MiniExcelLibs;
using Microsoft.EntityFrameworkCore;

namespace AedLocator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly AedContext _context;

        public ImportController(AedContext context)
        {
            _context = context;
        }

        // 1. Added [Consumes] so Swagger shows the "Choose File" button
        // 2. Added IFormFile so the method accepts a file upload
        [HttpPost("upload-excel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            // Check if a file was actually uploaded
            if (file == null || file.Length == 0)
                return BadRequest("Please select an Excel file to upload.");

            try
            {
                // We use file.OpenReadStream() to read the uploaded file directly from memory
                using (var stream = file.OpenReadStream())
                {
                    // MiniExcel reads the stream
                    var rows = MiniExcel.Query(stream).Cast<IDictionary<string, object>>();

                    foreach (var row in rows)
                    {
                        // Skip the header row
                        if (row["A"]?.ToString() == "Site" || row["A"]?.ToString() == "ID") continue;

                        var aed = new Aed
                        {
                            Site = row["A"]?.ToString(),
                            Organization = row["B"]?.ToString(),
                            Division = row["C"]?.ToString(),
                            Address = row["D"]?.ToString(),
                            City = row["E"]?.ToString(),
                            Province = row["F"]?.ToString(),
                            PostalCode = row["G"]?.ToString(),
                            Placement = row["H"]?.ToString(),
                            Manufacturer = row["I"]?.ToString(),
                            Model = row["J"]?.ToString()
                        };

                        // Only add if there is data in the row
                        if (!string.IsNullOrEmpty(aed.Site) || !string.IsNullOrEmpty(aed.Address))
                        {
                            _context.Aeds.Add(aed);
                        }
                    }
                }

                // Saves the data to the database (Azure SQL if your appsettings is updated!)
                await _context.SaveChangesAsync();

                return Ok(new { message = "Successfully imported data to the database!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}