using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RentalAuto.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    [Required(ErrorMessage = "Название услуги обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 50 символов")]
    [Display(Name = "Услуга")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Цена услуги обязательна")]
    [Range(0.01, 10000, ErrorMessage = "Цена должна быть от 0.01 до 10000")]
    [DataType(DataType.Currency)]
    [Display(Name = "Цена")]
    public decimal Price { get; set; }

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