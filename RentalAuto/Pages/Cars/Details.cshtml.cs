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
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public DetailsModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

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

            // Если клиент просматривает недоступный автомобиль
            if (User.IsInRole("rental_client") && car.Status != "available")
            {
                return Forbid();
            }

            Car = car;
            return Page();
        }
    }
}