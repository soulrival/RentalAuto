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

namespace RentalAuto.Pages.Clients
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
        public Client Client { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверка на уникальность email и паспорта
            var existingEmail = await _context.Clients
                .AnyAsync(c => c.Email == Client.Email);
            var existingPassport = await _context.Clients
                .AnyAsync(c => c.PassportNumber == Client.PassportNumber);
            var existingLicense = await _context.Clients
                .AnyAsync(c => c.DriverLicenseNumber == Client.DriverLicenseNumber);

            if (existingEmail)
            {
                ModelState.AddModelError("Client.Email", "Клиент с таким email уже существует");
                return Page();
            }

            if (existingPassport)
            {
                ModelState.AddModelError("Client.PassportNumber", "Клиент с таким номером паспорта уже существует");
                return Page();
            }

            if (existingLicense)
            {
                ModelState.AddModelError("Client.DriverLicenseNumber", "Клиент с таким номером водительского удостоверения уже существует");
                return Page();
            }

            _context.Clients.Add(Client);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}