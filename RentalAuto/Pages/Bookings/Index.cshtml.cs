using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Bookings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get; set; } = default!;

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

            IQueryable<Booking> bookingsIQ = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Client)
                .Include(b => b.PickupLocation)
                .Include(b => b.ReturnLocation)
                .AsQueryable();

            // Для клиента показываем только его бронирования
            if (User.IsInRole("rental_client"))
            {
                string userName = User.Identity?.Name ?? string.Empty;
                var currentClient = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Email == userName);

                if (currentClient != null)
                {
                    bookingsIQ = bookingsIQ.Where(b => b.ClientId == currentClient.ClientId);
                }
                else
                {
                    bookingsIQ = bookingsIQ.Where(b => false); // Пустая коллекция
                }
            }

            // Фильтрация по поисковой строке
            if (!string.IsNullOrEmpty(searchString))
            {
                bookingsIQ = bookingsIQ.Where(s =>
                    s.Car.Brand.Contains(searchString) ||
                    s.Car.ModelName.Contains(searchString) ||
                    s.Client.Surname.Contains(searchString) ||
                    s.Client.Firstname.Contains(searchString));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "car_asc":
                    bookingsIQ = bookingsIQ.OrderBy(s => s.Car.Brand + " " + s.Car.ModelName);
                    break;
                case "car_desc":
                    bookingsIQ = bookingsIQ.OrderByDescending(s => s.Car.Brand + " " + s.Car.ModelName);
                    break;
                case "client_asc":
                    bookingsIQ = bookingsIQ.OrderBy(s => s.Client.Surname + " " + s.Client.Firstname);
                    break;
                case "client_desc":
                    bookingsIQ = bookingsIQ.OrderByDescending(s => s.Client.Surname + " " + s.Client.Firstname);
                    break;
                case "date_asc":
                    bookingsIQ = bookingsIQ.OrderBy(s => s.StartDate);
                    break;
                case "date_desc":
                    bookingsIQ = bookingsIQ.OrderByDescending(s => s.StartDate);
                    break;
                case "price_asc":
                    bookingsIQ = bookingsIQ.OrderBy(s => s.TotalPrice);
                    break;
                case "price_desc":
                    bookingsIQ = bookingsIQ.OrderByDescending(s => s.TotalPrice);
                    break;
                default:
                    bookingsIQ = bookingsIQ.OrderBy(s => s.StartDate);
                    break;
            }

            TotalItems = await bookingsIQ.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            CurrentPage = pageIndex ?? 1;
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            Booking = await bookingsIQ
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}