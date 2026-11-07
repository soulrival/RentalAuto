using System.ComponentModel.DataAnnotations;

namespace RentalAuto.Models.Validation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date < DateTime.Today.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Дата не может быть в прошлом");
                }
            }
            return ValidationResult.Success;
        }
    }
}