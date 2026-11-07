using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Cars
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<Car> Car { get; set; } = default!;

        // Параметры сортировки
        public string? SortOrder { get; set; }
        public string? CurrentFilter { get; set; }
        public string? CurrentSort { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        // Для поиска
        public string? SearchString { get; set; }

        public Car Car1
        {
            get => default;
            set
            {
            }
        }

        public async Task OnGetAsync(string sortOrder, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            SortOrder = sortOrder == "asc" ? "desc" : "asc";
            CurrentFilter = searchString;
            SearchString = searchString;

            IQueryable<Car> carsIQ = _context.Cars.AsQueryable();

            // Для клиента показываем только доступные автомобили
            if (User.IsInRole("rental_client"))
            {
                carsIQ = carsIQ.Where(c => c.Status == "available");
            }

            // Фильтрация по поисковой строке
            if (!string.IsNullOrEmpty(searchString))
            {
                carsIQ = carsIQ.Where(s => s.Brand.Contains(searchString) ||
                                          s.ModelName.Contains(searchString) ||
                                          s.RegistrationNumber.Contains(searchString));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "brand_asc":
                    carsIQ = carsIQ.OrderBy(s => s.Brand);
                    break;
                case "brand_desc":
                    carsIQ = carsIQ.OrderByDescending(s => s.Brand);
                    break;
                case "model_asc":
                    carsIQ = carsIQ.OrderBy(s => s.ModelName);
                    break;
                case "model_desc":
                    carsIQ = carsIQ.OrderByDescending(s => s.ModelName);
                    break;
                case "year_asc":
                    carsIQ = carsIQ.OrderBy(s => s.Year);
                    break;
                case "year_desc":
                    carsIQ = carsIQ.OrderByDescending(s => s.Year);
                    break;
                case "price_asc":
                    carsIQ = carsIQ.OrderBy(s => s.PricePerDay);
                    break;
                case "price_desc":
                    carsIQ = carsIQ.OrderByDescending(s => s.PricePerDay);
                    break;
                case "status_asc":
                    carsIQ = carsIQ.OrderBy(s => s.Status);
                    break;
                case "status_desc":
                    carsIQ = carsIQ.OrderByDescending(s => s.Status);
                    break;
                default:
                    carsIQ = carsIQ.OrderBy(s => s.Brand);
                    break;
            }

            TotalItems = await carsIQ.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            CurrentPage = pageIndex ?? 1;
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            Car = await carsIQ
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}