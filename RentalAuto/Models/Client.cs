using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalAuto.Models;

public partial class Client
{
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 50 символов")]
    [Display(Name = "Фамилия")]
    public string Surname { get; set; } = null!;

    [Required(ErrorMessage = "Имя обязательно для заполнения")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов")]
    [Display(Name = "Имя")]
    public string Firstname { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Отчество должно содержать не более 50 символов")]
    [Display(Name = "Отчество")]
    public string? Patronymic { get; set; }

    [Required(ErrorMessage = "Дата рождения обязательна")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата рождения")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Телефон обязателен")]
    [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "Неверный формат телефона. Пример: +79001234567")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Неверный формат email адреса")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Номер паспорта обязателен")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Номер паспорта должен содержать 10 цифр")]
    [Display(Name = "Номер паспорта")]
    public string PassportNumber { get; set; } = null!;

    [Required(ErrorMessage = "Номер водительского удостоверения обязателен")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Номер водительского удостоверения должен содержать 10 цифр")]
    [Display(Name = "Водительское удостоверение")]
    public string DriverLicenseNumber { get; set; } = null!;

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
