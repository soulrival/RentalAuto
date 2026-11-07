using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Locations
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<Location> Location { get; set; } = default!;

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

            IQueryable<Location> locationsIQ = _context.Locations.AsQueryable();

            // Фильтрация по поисковой строке
            if (!string.IsNullOrEmpty(searchString))
            {
                locationsIQ = locationsIQ.Where(s => s.Name.Contains(searchString) ||
                                          s.City.Contains(searchString) ||
                                          s.Address.Contains(searchString));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "name_asc":
                    locationsIQ = locationsIQ.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    locationsIQ = locationsIQ.OrderByDescending(s => s.Name);
                    break;
                case "city_asc":
                    locationsIQ = locationsIQ.OrderBy(s => s.City);
                    break;
                case "city_desc":
                    locationsIQ = locationsIQ.OrderByDescending(s => s.City);
                    break;
                case "address_asc":
                    locationsIQ = locationsIQ.OrderBy(s => s.Address);
                    break;
                case "address_desc":
                    locationsIQ = locationsIQ.OrderByDescending(s => s.Address);
                    break;
                default:
                    locationsIQ = locationsIQ.OrderBy(s => s.Name);
                    break;
            }

            TotalItems = await locationsIQ.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            CurrentPage = pageIndex ?? 1;
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            Location = await locationsIQ
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}