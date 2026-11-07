using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Services
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<Service> Service { get; set; } = default!;

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

            IQueryable<Service> servicesIQ = _context.Services.AsQueryable();

            // Фильтрация по поисковой строке
            if (!string.IsNullOrEmpty(searchString))
            {
                servicesIQ = servicesIQ.Where(s => s.Name.Contains(searchString));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "name_asc":
                    servicesIQ = servicesIQ.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    servicesIQ = servicesIQ.OrderByDescending(s => s.Name);
                    break;
                case "price_asc":
                    servicesIQ = servicesIQ.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    servicesIQ = servicesIQ.OrderByDescending(s => s.Price);
                    break;
                default:
                    servicesIQ = servicesIQ.OrderBy(s => s.Name);
                    break;
            }

            TotalItems = await servicesIQ.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            CurrentPage = pageIndex ?? 1;
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            Service = await servicesIQ
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}