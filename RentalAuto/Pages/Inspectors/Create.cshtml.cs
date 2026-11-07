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
        public Inspector Inspector { get; set; } = default!;

        // Для валидации даты
        [BindProperty]
        public DateTime HireDate { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Устанавливаем дату приема
            Inspector.HireDate = HireDate;

            // Проверка на уникальность по ФИО и дате приема
            var existingInspector = await _context.Inspectors
                .AnyAsync(i => i.Surname == Inspector.Surname &&
                             i.Firstname == Inspector.Firstname &&
                             i.HireDate.Date == Inspector.HireDate.Date);

            if (existingInspector)
            {
                ModelState.AddModelError(string.Empty, "Осмотрщик с такими ФИО и датой приема уже существует");
                return Page();
            }

            _context.Inspectors.Add(Inspector);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}