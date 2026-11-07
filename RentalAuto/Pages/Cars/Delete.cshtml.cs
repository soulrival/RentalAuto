using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Cars
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
        public Car Car { get; set; } = default!;

        public Car Car1
        {
            get => default;
            set
            {
            }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Cars == null)  // Исправлено: Cars вместо Car
            {
                return NotFound();
            }

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.CarId == id);  // Исправлено: Cars вместо Car
            if (car == null)
            {
                return NotFound();
            }

            Car = car;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Cars == null)  // Исправлено: Cars вместо Car
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);  // Исправлено: Cars вместо Car
            if (car != null)
            {
                // Проверяем, нет ли активных бронирований для этого автомобиля
                var hasBookings = await _context.Bookings
                    .AnyAsync(b => b.CarId == id);

                if (hasBookings)
                {
                    ModelState.AddModelError(string.Empty,
                        "Невозможно удалить автомобиль. Сначала удалите все связанные бронирования.");
                    return Page();
                }

                _context.Cars.Remove(car);  // Исправлено: Cars вместо Car
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}