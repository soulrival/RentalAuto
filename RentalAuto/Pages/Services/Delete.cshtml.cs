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
    public class DeleteModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public DeleteModel(RentalAuto.Models.RentalDbContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                // Проверяем, используется ли услуга в бронированиях
                var hasBookings = await _context.Bookings
                    .AnyAsync(b => b.Services.Any(s => s.ServiceId == id));

                if (hasBookings)
                {
                    ModelState.AddModelError(string.Empty,
                        "Невозможно удалить услугу. Она используется в существующих бронированиях.");
                    return Page();
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}