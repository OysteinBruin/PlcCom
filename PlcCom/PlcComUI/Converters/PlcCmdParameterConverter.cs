using System;
using System.Globalization;
using System.Windows.Data;

namespace PlcComUI.Converters
{
    public class PlcCmdParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            return Values.Clone();
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
