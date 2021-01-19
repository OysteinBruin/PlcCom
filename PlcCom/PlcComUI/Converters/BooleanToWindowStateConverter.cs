using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PlcComUI.Converters
{
    public class BooleanToWindowStateConverter : IValueConverter
    {
        public WindowState Maximized { get; set; } = WindowState.Maximized;

        public WindowState Normal { get; set; } = WindowState.Normal;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = (bool)value;
            return state ? Maximized : Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WindowState state = (WindowState)value;
            if (state == WindowState.Minimized)
            {
                return true;
            }
            else return false;
        }
    }
}
