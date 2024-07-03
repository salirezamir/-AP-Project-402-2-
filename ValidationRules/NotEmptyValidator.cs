using Microsoft.IdentityModel.Tokens;
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
    public class NotEmptyValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is string str)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return ValidationResult.ValidResult;
                }
            }
            if (value is ComboBoxItem item)
            {
                if (item.Content is string text)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        return ValidationResult.ValidResult;
                    }
                }
            }
            return new ValidationResult(false, "Field cannot be empty");
        }
    }
}
