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
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public EditModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Car Car { get; set; } = default!;

        // Для выпадающих списков
        public SelectList ClassList => new SelectList(new[] { "Economy", "Standard", "SUV", "Premium" });
        public SelectList TransmissionList => new SelectList(new[] { "manual", "automatic", "CVT" });
        public SelectList FuelTypeList => new SelectList(new[] { "petrol", "diesel", "electric", "hybrid" });
        public SelectList StatusList => new SelectList(new[] { "available", "unavailable" });

        public bool IsInspector { get; set; }

        public Car Car1
        {
            get => default;
            set
            {
            }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.CarId == id);
            if (car == null)
            {
                return NotFound();
            }

            Car = car;
            IsInspector = User.IsInRole("rental_inspector") && !User.IsInRole("rental_admin");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                IsInspector = User.IsInRole("rental_inspector") && !User.IsInRole("rental_admin");
                return Page();
            }

            try
            {
                _context.Attach(Car).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(Car.CarId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // После редактирования осмотрщиком перенаправляем на страницу инспекции
            if (User.IsInRole("rental_inspector") && !User.IsInRole("rental_admin"))
            {
                return RedirectToPage("/Inspectors/Index");
            }

            return RedirectToPage("./Index");
        }

        private bool CarExists(int id)
        {
            return (_context.Cars?.Any(e => e.CarId == id)).GetValueOrDefault();
        }
    }
}