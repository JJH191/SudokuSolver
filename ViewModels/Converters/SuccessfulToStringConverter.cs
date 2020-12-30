using System;
using System.Globalization;
using System.Windows.Data;

namespace ViewModels.Converters
{
    public class SuccessfulToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Successful" : "Unsuccessful";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
