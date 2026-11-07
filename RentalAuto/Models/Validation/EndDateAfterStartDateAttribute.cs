using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RentalAuto.Models.Validation
{
    public class EndDateAfterStartDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var endDate = (DateTime?)value;
            if (endDate == null) return ValidationResult.Success;

            var startDateProperty = validationContext.ObjectType.GetProperty("StartDate");
            if (startDateProperty == null)
                return new ValidationResult("Свойство StartDate не найдено.");

            var startDate = (DateTime?)startDateProperty.GetValue(validationContext.ObjectInstance);

            if (startDate.HasValue && endDate.Value.Date <= startDate.Value.Date)
            {
                return new ValidationResult(ErrorMessage ?? "Дата окончания должна быть позже даты начала.");
            }

            return ValidationResult.Success;
        }
    }
}