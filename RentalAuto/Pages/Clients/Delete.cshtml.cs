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
    [Authorize(Policy = "RentalAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public DeleteModel(RentalAuto.Models.RentalDbContext context)
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

            Client = client;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                // Проверяем, есть ли бронирования у этого клиента
                var hasBookings = await _context.Bookings
                    .AnyAsync(b => b.ClientId == id);

                if (hasBookings)
                {
                    ModelState.AddModelError(string.Empty,
                        "Невозможно удалить клиента. Сначала удалите все его бронирования.");
                    return Page();
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}