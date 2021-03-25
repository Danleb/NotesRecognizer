using System.Globalization;
using System.Windows.Controls;

namespace VoiceChangerApp.Views.ValidationRules
{
    public class IntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object data, CultureInfo cultureInfo)
        {
            return Validate(data, cultureInfo, out _);
        }

        public ValidationResult Validate(object data, CultureInfo cultureInfo, out int value)
        {
            var s = (string)data;
            value = 0;

            if (!double.TryParse(s, out _))
                return new ValidationResult(false, "Not a number entered.");

            if (!int.TryParse(s, out value))
                return new ValidationResult(false, "Not an integer number entered.");

            return ValidationResult.ValidResult;
        }
    }
}