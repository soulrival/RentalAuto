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
    public class CreateModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public CreateModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверка на уникальность телефона
            var existingPhone = await _context.Locations
                .AnyAsync(l => l.Phone == Location.Phone);

            if (existingPhone)
            {
                ModelState.AddModelError("Location.Phone", "Локация с таким телефоном уже существует");
                return Page();
            }

            _context.Locations.Add(Location);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}