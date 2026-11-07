using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RentalAuto.Models;

/// <summary>
/// Хранение информации об автомобилях автопарка
/// </summary>
public partial class Car
{
    public int CarId { get; set; }

    [Required(ErrorMessage = "VIN обязателен")]
    [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "Неверный формат VIN-номера")]
    [StringLength(17, ErrorMessage = "VIN должен содержать 17 символов")]
    [Display(Name = "VIN")]
    public string Vin { get; set; } = null!;

    [Required(ErrorMessage = "Регистрационный номер обязателен")]
    [RegularExpression(@"^[АВЕКМНОРСТУХABEKMHOPCTYX]{1}[0-9]{3}[АВЕКМНОРСТУХABEKMHOPCTYX]{2}\s?[0-9]{2,3}$",
        ErrorMessage = "Неверный формат регистрационного номера")]
    [StringLength(12, ErrorMessage = "Регистрационный номер слишком длинный")]
    [Display(Name = "Регистрационный номер")]
    public string RegistrationNumber { get; set; } = null!;

    [Required(ErrorMessage = "Марка обязательна")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Марка должна содержать от 2 до 100 символов")]
    [Display(Name = "Марка")]
    public string Brand { get; set; } = null!;

    [Required(ErrorMessage = "Модель обязательна")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Модель должна содержать от 2 до 100 символов")]
    [Display(Name = "Модель")]
    public string ModelName { get; set; } = null!;

    [Required(ErrorMessage = "Класс автомобиля обязателен")]
    [StringLength(20)]
    [RegularExpression(@"^(Economy|Standard|SUV|Premium)$",
        ErrorMessage = "Допустимые значения: Economy, Standard, SUV, Premium")]
    [Display(Name = "Класс автомобиля")]
    public string Class { get; set; } = null!;

    [Required(ErrorMessage = "Год выпуска обязателен")]
    [Range(1980, 2030, ErrorMessage = "Год выпуска должен быть между 1980 и 2030")]
    [Display(Name = "Год выпуска")]
    public int Year { get; set; }

    [StringLength(50, ErrorMessage = "Двигатель должен содержать не более 50 символов")]
    [Display(Name = "Двигатель")]
    public string? Engine { get; set; }

    [Required(ErrorMessage = "Тип коробки передач обязателен")]
    [StringLength(20)]
    [RegularExpression(@"^(manual|automatic|CVT)$",
        ErrorMessage = "Допустимые значения: manual, automatic, CVT")]
    [Display(Name = "Коробка передач")]
    public string Transmission { get; set; } = null!;

    [Required(ErrorMessage = "Количество мест обязательно")]
    [Range(2, 9, ErrorMessage = "Количество мест должно быть от 2 до 9")]
    [Display(Name = "Количество мест")]
    public int Seats { get; set; }

    [Required(ErrorMessage = "Пробег обязателен")]
    [Range(0, int.MaxValue, ErrorMessage = "Пробег не может быть отрицательным")]
    [Display(Name = "Пробег")]
    public int Mileage { get; set; }

    [Required(ErrorMessage = "Тип топлива обязателен")]
    [StringLength(20)]
    [RegularExpression(@"^(petrol|diesel|electric|hybrid)$",
        ErrorMessage = "Допустимые значения: petrol, diesel, electric, hybrid")]
    [Display(Name = "Тип топлива")]
    public string FuelType { get; set; } = null!;

    /// <summary>
    /// Статус доступности: available/unavailable
    /// </summary>
    [Required(ErrorMessage = "Статус обязателен")]
    [StringLength(20)]
    [RegularExpression(@"^(available|unavailable)$",
        ErrorMessage = "Допустимые значения: available, unavailable")]
    [Display(Name = "Статус")]
    public string Status { get; set; } = null!;

    [Required(ErrorMessage = "Цена в день обязательна")]
    [Range(0.01, 100000, ErrorMessage = "Цена должна быть от 0.01 до 100000")]
    [DataType(DataType.Currency)]
    [Display(Name = "Цена в день")]
    public decimal PricePerDay { get; set; }

    [StringLength(1000, ErrorMessage = "Описание должно содержать не более 1000 символов")]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public Booking Booking
    {
        get => default;
        set
        {
        }
    }

    public BookingDetail BookingDetail
    {
        get => default;
        set
        {
        }
    }
}