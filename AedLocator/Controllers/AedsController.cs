using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AedLocator.Models;

namespace AedLocator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AedsController : ControllerBase
    {
        private readonly AedContext _context;

        public AedsController(AedContext context)
        {
            _context = context;
        }

        // 1. SEARCH FUNCTION: Handles City and/or Postal Code
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Aed>>> Search(string? city, string? postalCode, string? keyword)
        {
            var query = _context.Aeds.AsQueryable();

            // GLOBAL SEARCH: Matches City, Site, OR Address
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(a => a.City.Contains(city)
                                      || a.Site.Contains(city)
                                      || a.Address.Contains(city));
            }

            if (!string.IsNullOrEmpty(postalCode))
            {
                var cleanInput = postalCode.Replace(" ", "");
                query = query.Where(a => a.PostalCode.Replace(" ", "").StartsWith(cleanInput));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.Site.Contains(keyword) || a.Placement.Contains(keyword));
            }

            return await query.ToListAsync();
        }

        // 2. IMPORT FUNCTION: Maps Excel Columns A-J to Database
        [HttpPost("upload-excel")]
        public async Task<IActionResult> ImportExcel()
        {
            var filePath = @"C:\Users\YIM CHUN HIN\Desktop\project\AED_Data.xlsx";

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found at the specified path.");

            try
            {
                var rows = MiniExcel.Query(filePath).Cast<IDictionary<string, object>>();
                int count = 0;

                foreach (var row in rows)
                {
                    // Skip the header row if Column A contains "Site" or "ID"
                    var firstCell = row["A"]?.ToString();
                    if (firstCell == "Site" || firstCell == "ID") continue;

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

                    // Only add if there is some data in the row
                    if (!string.IsNullOrEmpty(aed.Site) || !string.IsNullOrEmpty(aed.Address))
                    {
                        _context.Aeds.Add(aed);
                        count++;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok($"Successfully imported {count} rows.");
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // 3. GET ALL (Optional: Useful for testing)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aed>>> GetAeds()
        {
            return await _context.Aeds.ToListAsync();
        }
    }
}