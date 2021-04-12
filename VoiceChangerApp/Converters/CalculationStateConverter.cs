using System;
using System.Globalization;
using System.Windows.Data;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Converters
{
    public class CalculationStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (CalculationState)value;
            switch (state)
            {
                case CalculationState.None: return "-";
                case CalculationState.Calculating: return "In progress";
                case CalculationState.Cancelled: return "Cancelled";
                case CalculationState.ErrorHappened: return "Error happened";
                case CalculationState.Finished: return "Calculated";
                default: throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
