using Restaurant_Manager.DAL;
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
    public class NumberValidator : ValidationRule
    {
        private RestaurantContext _context = new RestaurantContext();
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string number)
            {
                string numberPattern = @"^09\d{9}$";
                if (Regex.IsMatch(number, numberPattern))
                {
                    if (_context.Users.Any(u => u.Phone == number))
                    {
                        return new ValidationResult(false, $"PhoneNumber already exists");
                    }
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, $"Invalid number format");
        }

    }
}
