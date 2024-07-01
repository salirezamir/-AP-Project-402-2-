using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Restaurant_Manager.ValidationRules
{
    public class EmailValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string email)
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (Regex.IsMatch(email, emailPattern))
                {
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, $"Invalid email format");
        }
    }
}
