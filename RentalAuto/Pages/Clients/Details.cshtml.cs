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
    public class DetailsModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public DetailsModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

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
    }
}