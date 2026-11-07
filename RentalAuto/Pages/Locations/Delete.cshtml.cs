using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Locations
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
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Locations == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FirstOrDefaultAsync(m => m.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            Location = location;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Locations == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                // Проверяем, используется ли локация в бронированиях
                var hasBookingsAsPickup = await _context.Bookings
                    .AnyAsync(b => b.PickupLocationId == id);
                var hasBookingsAsReturn = await _context.Bookings
                    .AnyAsync(b => b.ReturnLocationId == id);

                if (hasBookingsAsPickup || hasBookingsAsReturn)
                {
                    ModelState.AddModelError(string.Empty,
                        "Невозможно удалить локацию. Она используется в существующих бронированиях.");
                    return Page();
                }

                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}