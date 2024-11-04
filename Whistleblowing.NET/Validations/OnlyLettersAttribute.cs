using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Whistleblowing.NET.Validations
{
    public class OnlyLettersAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string input = value.ToString();
                if (!Regex.IsMatch(input, @"^[a-zA-Z\s]+$")) // Permette solo lettere e spazi
                {
                    return new ValidationResult("Il campo può contenere solo lettere.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
