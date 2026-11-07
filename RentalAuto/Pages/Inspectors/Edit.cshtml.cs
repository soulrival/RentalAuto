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
    public class EditModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public EditModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Inspector Inspector { get; set; } = default!;

        // Для валидации даты
        [BindProperty]
        public DateTime HireDate { get; set; }

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
            HireDate = inspector.HireDate;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Устанавливаем дату приема
            Inspector.HireDate = HireDate;

            _context.Attach(Inspector).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InspectorExists(Inspector.InspectorId))
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

        private bool InspectorExists(int id)
        {
            return (_context.Inspectors?.Any(e => e.InspectorId == id)).GetValueOrDefault();
        }
    }
}