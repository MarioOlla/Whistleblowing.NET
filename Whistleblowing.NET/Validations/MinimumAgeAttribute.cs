using System.ComponentModel.DataAnnotations;

namespace Whistleblowing.NET.Validations
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;

        }



        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                //Calcolo l' età basandomi sull' anno mese e giorno

                DateTime today = DateTime.Today;
                int age = today.Year - dateOfBirth.Year;

                if (dateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"L' età minima richiesta è di {_minimumAge} anni");
                }
            }

            return ValidationResult.Success;
        }
    }

}
