using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Inspectors
{
    [Authorize(Policy = "RentalAdmin")]
    public class DetailsModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public DetailsModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public Inspector Inspector { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Inspectors == null)
            {
                return NotFound();
            }

            var inspector = await _context.Inspectors.FirstOrDefaultAsync(m => m.InspectorId == id);
            if (inspector == null)
            {
                return NotFound();
            }

            Inspector = inspector;
            return Page();
        }
    }
}