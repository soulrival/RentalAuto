using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Cars
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
        public Car Car { get; set; } = default!;

        // Для выпадающих списков
        public SelectList ClassList => new SelectList(new[] { "Economy", "Standard", "SUV", "Premium" });
        public SelectList TransmissionList => new SelectList(new[] { "manual", "automatic", "CVT" });
        public SelectList FuelTypeList => new SelectList(new[] { "petrol", "diesel", "electric", "hybrid" });
        public SelectList StatusList => new SelectList(new[] { "available", "unavailable" });

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Cars.Add(Car);  // Исправлено: Cars вместо Car
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public Car Car1
        {
            get => default;
            set
            {
            }
        }
    }
}