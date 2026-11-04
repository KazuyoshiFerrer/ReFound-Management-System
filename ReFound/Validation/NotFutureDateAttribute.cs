using System;
using System.ComponentModel.DataAnnotations;

namespace ReFound.Validation
{
    public class NotFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue > DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }
}