using System;
using System.ComponentModel.DataAnnotations;

namespace RentalAuto.Models;

public partial class BookingDetail
{
    public int? BookingId { get; set; }

    [Display(Name = "Клиент")]
    public string? ClientFullname { get; set; }

    [Display(Name = "Автомобиль")]
    public string? CarModel { get; set; }

    [Display(Name = "Локация получения")]
    public string? PickupLocation { get; set; }

    [Display(Name = "Локация возврата")]
    public string? ReturnLocation { get; set; }

    [Display(Name = "Дата начала")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? StartDate { get; set; }

    [Display(Name = "Дата окончания")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? EndDate { get; set; }

    [Display(Name = "Дополнительные услуги")]
    public string? Services { get; set; }

    [Display(Name = "Итоговая цена")]
    [DataType(DataType.Currency)]
    public decimal? TotalPrice { get; set; }
}