using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalAuto.Models
{
    public partial class RentalDbContext
    {
        // Метод для получения доступных автомобилей
        public IQueryable<Car> GetAvailableCars()
        {
            return Cars.Where(c => c.Status == "available");
        }

        // Метод расчёта стоимости бронирования с DateTime
        public async Task<decimal> CalculateBookingPriceAsync(
            int carId,
            DateTime startDate,
            DateTime endDate,
            List<int> serviceIds)
        {
            var car = await Cars.FindAsync(carId);
            if (car == null)
                throw new ArgumentException("Автомобиль не найден", nameof(carId));

            var services = await Services
                .Where(s => serviceIds.Contains(s.ServiceId))
                .ToListAsync();

            // Вычисляем количество полных дней
            int days = (int)(endDate.Date - startDate.Date).TotalDays;
            if (days <= 0) days = 1; // Минимум 1 день

            decimal basePrice = car.PricePerDay * days;
            decimal servicesPrice = services.Sum(s => s.Price);

            return basePrice + servicesPrice;
        }
    }
}