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
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<Client> Client { get; set; } = default!;

        // Параметры сортировки
        public string? SortOrder { get; set; }
        public string? CurrentFilter { get; set; }
        public string? CurrentSort { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public string? SearchString { get; set; }
        public bool IsClientRole { get; set; }

        public async Task OnGetAsync(string sortOrder, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            SortOrder = sortOrder == "asc" ? "desc" : "asc";
            CurrentFilter = searchString;
            SearchString = searchString;

            IQueryable<Client> clientsIQ = _context.Clients.AsQueryable();

            // Если текущий пользователь - клиент, показываем только его данные
            IsClientRole = User.IsInRole("rental_client");
            if (IsClientRole)
            {
                // Получаем имя пользователя безопасно
                string userName = User.Identity?.Name ?? string.Empty;
                var currentClient = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Email == userName);

                if (currentClient != null)
                {
                    clientsIQ = clientsIQ.Where(c => c.ClientId == currentClient.ClientId);
                }
                else
                {
                    clientsIQ = clientsIQ.Where(c => false); // Пустая коллекция
                }
            }

            // Фильтрация по поисковой строке
            if (!string.IsNullOrEmpty(searchString))
            {
                clientsIQ = clientsIQ.Where(s => s.Surname.Contains(searchString) ||
                                          s.Firstname.Contains(searchString) ||
                                          s.Email.Contains(searchString) ||
                                          s.PassportNumber.Contains(searchString));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "surname_asc":
                    clientsIQ = clientsIQ.OrderBy(s => s.Surname);
                    break;
                case "surname_desc":
                    clientsIQ = clientsIQ.OrderByDescending(s => s.Surname);
                    break;
                case "name_asc":
                    clientsIQ = clientsIQ.OrderBy(s => s.Firstname);
                    break;
                case "name_desc":
                    clientsIQ = clientsIQ.OrderByDescending(s => s.Firstname);
                    break;
                case "date_asc":
                    clientsIQ = clientsIQ.OrderBy(s => s.DateOfBirth);
                    break;
                case "date_desc":
                    clientsIQ = clientsIQ.OrderByDescending(s => s.DateOfBirth);
                    break;
                case "email_asc":
                    clientsIQ = clientsIQ.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    clientsIQ = clientsIQ.OrderByDescending(s => s.Email);
                    break;
                default:
                    clientsIQ = clientsIQ.OrderBy(s => s.Surname);
                    break;
            }

            TotalItems = await clientsIQ.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            CurrentPage = pageIndex ?? 1;
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            Client = await clientsIQ
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}