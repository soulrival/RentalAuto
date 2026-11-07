using RentalAuto.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalAuto.Models;

public partial class Inspector
{

    public int InspectorId { get; set; }

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

    [Required(ErrorMessage = "Дата приема на работу обязательна")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата приема")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime HireDate { get; set; }

    public Car Car
    {
        get => default;
        set
        {
        }
    }
}