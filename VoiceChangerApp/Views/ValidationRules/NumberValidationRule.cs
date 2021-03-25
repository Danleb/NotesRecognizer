using System.Globalization;
using System.Windows.Controls;

namespace VoiceChangerApp.Views.ValidationRules
{
    public class NumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Validate(value, cultureInfo, out _);
        }

        public ValidationResult Validate(object data, CultureInfo cultureInfo, out double value)
        {
            var s = (string)data;

            if (!double.TryParse(s, out value))
                return new ValidationResult(false, "Not a number entered.");

            return ValidationResult.ValidResult;
        }
    }
}