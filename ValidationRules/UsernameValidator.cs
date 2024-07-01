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

    class UsernameValidator : ValidationRule
    {
        private RestaurantContext _context = new RestaurantContext();
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string username)
            {
                string usernamePattern = @"^[a-zA-Z0-9._%+-]{5,}$";
                if (Regex.IsMatch(username, usernamePattern))
                {
                    if (_context.Users.Any(u => u.Username == username))
                    {
                        return new ValidationResult(false, $"Username already exists");
                    }
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, $"at least 5 characters long and Don't use special Chars");
        }
    }
}
