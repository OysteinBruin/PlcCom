using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PlcComUI.Converters
{
    public class BooleanGreenRedColorConverter : IValueConverter
    {
        public Brush HighValue { get; set; } = Brushes.LimeGreen;

        public Brush LowValue { get; set; } = Brushes.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = (bool)value;
            return state ? HighValue : LowValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
