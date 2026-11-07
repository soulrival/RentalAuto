using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RentalAuto.Models;

public partial class Location
{
    public int LocationId { get; set; }

    [Required(ErrorMessage = "Название локации обязательно")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Название должно содержать от 3 до 100 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Адрес обязателен")]
    [StringLength(255, MinimumLength = 5, ErrorMessage = "Адрес должен содержать от 5 до 255 символов")]
    [Display(Name = "Адрес")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Город обязателен")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Город должен содержать от 2 до 100 символов")]
    [Display(Name = "Город")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Телефон обязателен")]
    [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "Неверный формат телефона. Пример: +79001234567")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Часы работы обязательны")]
    [StringLength(50, ErrorMessage = "Часы работы должны содержать не более 50 символов")]
    [Display(Name = "Часы работы")]
    public string WorkingHours { get; set; } = null!;

    public virtual ICollection<Booking> BookingPickupLocations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingReturnLocations { get; set; } = new List<Booking>();

    public Booking BookingReturn
    {
        get => default;
        set
        {
        }
    }

    public Booking BookingPickup
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
