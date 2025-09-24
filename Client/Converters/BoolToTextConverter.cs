using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Client.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var options = (parameter as string)?.Split('|');
            if (value is bool b && options?.Length == 2)
                return b ? options[0] : options[1];
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}