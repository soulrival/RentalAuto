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
        public Service Service { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверка на уникальность названия услуги
            var existingService = await _context.Services
                .AnyAsync(s => s.Name == Service.Name);

            if (existingService)
            {
                ModelState.AddModelError("Service.Name", "Услуга с таким названием уже существует");
                return Page();
            }

            _context.Services.Add(Service);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}