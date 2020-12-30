using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlcUnitTestUI.Converters
{
    internal class ExpanderDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is ExpandDirection direction &&
            //    targetType is { } &&
            //    parameter is string values &&
            //    values.Split(',') is { } directionValues &&
            //    directionValues.Length == 4)
            //{
            //    int index = direction switch
            //    {
            //        ExpandDirection.Left => 0,
            //        ExpandDirection.Up => 1,
            //        ExpandDirection.Right => 2,
            //        ExpandDirection.Down => 3,
            //        _ => throw new InvalidOperationException()
            //    };
            //    var converter = TypeDescriptor.GetConverter(targetType);

            //    return converter.CanConvertFrom(typeof(string)) ?
            //           converter.ConvertFromInvariantString(directionValues[index]) :
            //           directionValues[index];
            //}
            //return Binding.DoNothing;

            string values = (string)parameter;
            string[] directionValues = values.Split(',');

            ExpandDirection direction = (ExpandDirection)value;

            int index = 0;
            switch (direction)
            {
                case ExpandDirection.Down:
                    index = 3;
                    break;
                case ExpandDirection.Up:
                    index = 1;
                    break;
                case ExpandDirection.Left:
                    index = 0;
                    break;
                case ExpandDirection.Right:
                    index = 2;
                    break;
            }

            

            var converter = TypeDescriptor.GetConverter(targetType);
            return converter.CanConvertFrom(typeof(string)) ?
                       converter.ConvertFromInvariantString(directionValues[index]) :
                       directionValues[index];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
