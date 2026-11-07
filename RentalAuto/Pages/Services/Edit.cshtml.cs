using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Services
{
    [Authorize(Policy = "RentalAdmin")]
    public class EditModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public EditModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Service Service { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            Service = service;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Service).State = EntityState.Modified;

            try
            {
                // Проверка на уникальность названия (игнорируя текущую запись)
                var existingService = await _context.Services
                    .AnyAsync(s => s.Name == Service.Name && s.ServiceId != Service.ServiceId);

                if (existingService)
                {
                    ModelState.AddModelError("Service.Name", "Услуга с таким названием уже существует");
                    return Page();
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(Service.ServiceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ServiceExists(int id)
        {
            return (_context.Services?.Any(e => e.ServiceId == id)).GetValueOrDefault();
        }
    }
}