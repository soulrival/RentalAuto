using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentalAuto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalAuto.Pages.Bookings
{
    [Authorize(Policy = "RentalClient")]
    public class CreateModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(RentalAuto.Models.RentalDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            try
            {
                var currentUserEmail = User.Identity?.Name;
                var currentClient = _context.Clients.FirstOrDefault(c => c.Email == currentUserEmail);

                if (currentClient == null)
                {
                    ModelState.AddModelError(string.Empty, "Клиент не найден в системе");
                    return RedirectToPage("/Account/Login");
                }

                // Заполняем выпадающие списки
                CarList = new SelectList(_context.Cars
                    .Where(c => c.Status == "available")
                    .Select(c => new { c.CarId, Name = $"{c.Brand} {c.ModelName} ({c.Class})" }),
                    "CarId", "Name");

                LocationList = new SelectList(_context.Locations
                    .Select(l => new { l.LocationId, Name = $"{l.Name} ({l.City})" }),
                    "LocationId", "Name");

                ServiceList = new MultiSelectList(_context.Services
                    .Select(s => new { s.ServiceId, Name = $"{s.Name} - {s.Price} ₽" }),
                    "ServiceId", "Name");

                // Устанавливаем значения по умолчанию для дат
                StartDate = DateTime.Today;
                EndDate = DateTime.Today.AddDays(3);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке страницы создания бронирования");
                ModelState.AddModelError(string.Empty, "Произошла ошибка при загрузке данных");
                return RedirectToPage("/Index");
            }
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        [BindProperty]
        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        public SelectList CarList { get; set; } = default!;
        public SelectList LocationList { get; set; } = default!;
        public MultiSelectList ServiceList { get; set; } = default!;

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(3);

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState не валиден");
                OnGet();
                return Page();
            }

            try
            {
                var currentUserEmail = User.Identity?.Name;
                var currentClient = await _context.Clients.FirstOrDefaultAsync(c => c.Email == currentUserEmail);

                if (currentClient == null)
                {
                    return Forbid();
                }

                // Проверяем доступность автомобиля
                var car = await _context.Cars.FindAsync(Booking.CarId);
                if (car == null || car.Status != "available")
                {
                    ModelState.AddModelError("Booking.CarId", "Выбранный автомобиль недоступен для бронирования");
                    OnGet();
                    return Page();
                }

                // Проверяем конфликты дат
                var hasConflicts = await _context.Bookings.AnyAsync(b =>
                    b.CarId == Booking.CarId &&
                    ((b.StartDate <= StartDate && b.EndDate >= StartDate) ||
                     (b.StartDate <= EndDate && b.EndDate >= EndDate) ||
                     (b.StartDate >= StartDate && b.EndDate <= EndDate)));

                if (hasConflicts)
                {
                    ModelState.AddModelError(string.Empty, "Автомобиль уже забронирован на выбранные даты");
                    OnGet();
                    return Page();
                }

                // Устанавливаем значения
                Booking.StartDate = StartDate.Date;
                Booking.EndDate = EndDate.Date;
                Booking.ClientId = currentClient.ClientId;

                if (Booking.PickupLocationId == 0)
                {
                    Booking.PickupLocationId = _context.Locations.First().LocationId;
                }
                if (Booking.ReturnLocationId == 0)
                {
                    Booking.ReturnLocationId = Booking.PickupLocationId;
                }

                Booking.TotalPrice = 0; // Будет рассчитано триггером
                Booking.ContractGenerated = false;

                _logger.LogInformation($"Создаем бронирование для клиента ID: {Booking.ClientId}, автомобиль ID: {Booking.CarId}");

                // Используем транзакцию для всех операций
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Добавляем бронирование
                    _context.Bookings.Add(Booking);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Бронирование создано с ID: {Booking.BookingId}");

                    // Добавляем услуги - корректный подход для составного ключа
                    if (SelectedServiceIds != null && SelectedServiceIds.Any())
                    {
                        foreach (var serviceId in SelectedServiceIds)
                        {
                            // Проверяем, существует ли уже такая запись
                            var exists = await _context.Database.ExecuteSqlRawAsync(@"
                                SELECT 1 FROM rental_schema.booking_service 
                                WHERE booking_id = {0} AND service_id = {1}",
                                Booking.BookingId, serviceId);

                            if (exists == 0) // Записи нет, можно вставлять
                            {
                                await _context.Database.ExecuteSqlRawAsync(@"
                                    INSERT INTO rental_schema.booking_service (booking_id, service_id) 
                                    VALUES ({0}, {1})",
                                    Booking.BookingId, serviceId);

                                _logger.LogInformation($"Услуга ID {serviceId} добавлена к бронированию {Booking.BookingId}");
                            }
                            else
                            {
                                _logger.LogWarning($"Услуга ID {serviceId} уже существует для бронирования {Booking.BookingId}");
                            }
                        }
                    }

                    // Принудительно обновляем для срабатывания триггера расчета
                    await _context.Database.ExecuteSqlRawAsync(@"
                        UPDATE rental_schema.booking 
                        SET total_price = total_price WHERE booking_id = {0}",
                        Booking.BookingId);

                    await transaction.CommitAsync();
                    _logger.LogInformation("Транзакция успешно завершена");

                    return RedirectToPage("./Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Ошибка при создании бронирования в транзакции");
                    throw; // Перебрасываем исключение для обработки ниже
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при создании бронирования");
                ModelState.AddModelError(string.Empty, $"Ошибка при создании бронирования: {ex.Message}");
                OnGet();
                return Page();
            }
        }
    }
}