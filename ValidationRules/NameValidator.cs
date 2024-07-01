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
    public class NameValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string name)
            {
                string namePattern = @"^[a-zA-Z ]{3,32}$";
                if (Regex.IsMatch(name, namePattern))
                {
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, $"Invalid name format");
        }
    }
}
