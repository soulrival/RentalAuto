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
    public class DeleteModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public DeleteModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Inspectors == null)
            {
                return NotFound();
            }

            var inspector = await _context.Inspectors.FindAsync(id);
            if (inspector != null)
            {
                // Проверяем, есть ли связанные записи (в будущем можно добавить проверку на проведенные осмотры)
                _context.Inspectors.Remove(inspector);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}