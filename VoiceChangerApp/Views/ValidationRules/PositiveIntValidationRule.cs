using System.Globalization;
using System.Windows.Controls;

namespace VoiceChangerApp.Views.ValidationRules
{
    public class PositiveIntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;

            var res = new IntegerValidationRule().Validate(value, cultureInfo);

            if (!res.IsValid)
                return res;

            var result = int.Parse(s);

            if (result <= 0)
                return new ValidationResult(false, "Not a positive number entered. Please enter a number bigger than zero.");

            return ValidationResult.ValidResult;
        }
    }
}