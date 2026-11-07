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
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<Inspector> Inspector { get; set; } = default!;

        // Параметры сортировки
        public string? SortOrder { get; set; }
        public string? CurrentFilter { get; set; }
        public string? CurrentSort { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public string? SearchString { get; set; }

        public async Task OnGetAsync(string sortOrder, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            SortOrder = sortOrder == "asc" ? "desc" : "asc";
            CurrentFilter = searchString;
            SearchString = searchString;

            IQueryable<Inspector> inspectorsIQ = _context.Inspectors.AsQueryable();

            // Фильтрация по поисковой строке
            if (!string.IsNullOrEmpty(searchString))
            {
                inspectorsIQ = inspectorsIQ.Where(s => s.Surname.Contains(searchString) ||
                                          s.Firstname.Contains(searchString) ||
                                          s.Patronymic.Contains(searchString));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "surname_asc":
                    inspectorsIQ = inspectorsIQ.OrderBy(s => s.Surname);
                    break;
                case "surname_desc":
                    inspectorsIQ = inspectorsIQ.OrderByDescending(s => s.Surname);
                    break;
                case "name_asc":
                    inspectorsIQ = inspectorsIQ.OrderBy(s => s.Firstname);
                    break;
                case "name_desc":
                    inspectorsIQ = inspectorsIQ.OrderByDescending(s => s.Firstname);
                    break;
                case "hiredate_asc":
                    inspectorsIQ = inspectorsIQ.OrderBy(s => s.HireDate);
                    break;
                case "hiredate_desc":
                    inspectorsIQ = inspectorsIQ.OrderByDescending(s => s.HireDate);
                    break;
                default:
                    inspectorsIQ = inspectorsIQ.OrderBy(s => s.Surname);
                    break;
            }

            TotalItems = await inspectorsIQ.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            CurrentPage = pageIndex ?? 1;
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            Inspector = await inspectorsIQ
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}