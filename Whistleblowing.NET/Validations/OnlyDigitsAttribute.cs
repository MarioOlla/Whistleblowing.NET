using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Whistleblowing.NET.Validations
{
    public class OnlyDigitsAttribute: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string phonenumber = value.ToString();

                //verifico che il numero contenga solo cifre

                if(!Regex.IsMatch(phonenumber, @"^\d+$"))
                {
                    return new ValidationResult("il numero di telefono puo contenere solo cifre");
                }
            }

            return ValidationResult.Success;
        }

    }
}
