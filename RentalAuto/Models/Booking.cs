using RentalAuto.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalAuto.Models;

/// <summary>
/// Информация о бронированиях автомобилей
/// </summary>
public partial class Booking
{
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Клиент обязателен")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Автомобиль обязателен")]
    public int CarId { get; set; }

    [Required(ErrorMessage = "Локация получения обязательна")]
    public int PickupLocationId { get; set; }

    [Required(ErrorMessage = "Локация возврата обязательна")]
    public int ReturnLocationId { get; set; }

    [Required(ErrorMessage = "Дата начала аренды обязательна")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата начала")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Дата окончания аренды обязательна")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата окончания")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [FutureDate(ErrorMessage = "Дата окончания не может быть в прошлом")]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Итоговая цена должна быть положительной")]
    [DataType(DataType.Currency)]
    [Display(Name = "Итоговая цена")]
    public decimal TotalPrice { get; set; }

    public bool ContractGenerated { get; set; }

    public virtual Car Car { get; set; } = null!;
    public virtual Client Client { get; set; } = null!;
    public virtual Location PickupLocation { get; set; } = null!;
    public virtual Location ReturnLocation { get; set; } = null!;
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public BookingDetail BookingDetail
    {
        get => default;
        set
        {
        }
    }
}