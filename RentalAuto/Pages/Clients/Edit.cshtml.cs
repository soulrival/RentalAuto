using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Clients
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
        public Client Client { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            // Если текущий пользователь - клиент, проверяем, что это его запись
            if (User.IsInRole("rental_client"))
            {
                // Безопасно получаем имя пользователя
                string userName = User.Identity?.Name ?? string.Empty;
                var currentUser = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Email == userName);

                if (currentUser?.ClientId != id)
                {
                    return Forbid();
                }
            }

            Client = client;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Client).State = EntityState.Modified;

            try
            {
                // Проверка на уникальность email и паспорта (игнорируя текущую запись)
                var existingEmail = await _context.Clients
                    .AnyAsync(c => c.Email == Client.Email && c.ClientId != Client.ClientId);
                var existingPassport = await _context.Clients
                    .AnyAsync(c => c.PassportNumber == Client.PassportNumber && c.ClientId != Client.ClientId);
                var existingLicense = await _context.Clients
                    .AnyAsync(c => c.DriverLicenseNumber == Client.DriverLicenseNumber && c.ClientId != Client.ClientId);

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

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(Client.ClientId))
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

        private bool ClientExists(int id)
        {
            return (_context.Clients?.Any(e => e.ClientId == id)).GetValueOrDefault();
        }
    }
}