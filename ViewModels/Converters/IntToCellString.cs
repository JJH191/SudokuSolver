using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ViewModels.Converters
{
    /// <summary>
    /// Converts a given bitmap to a bitmapImage
    /// Used to display a bitmap in a control through WPF
    /// </summary>
    public class IntToCellString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number = (int)value;
            if (number < 1 || number > 9) return "";
            else return number.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string number = (string)value;
            if (number.Trim().Length == 0) return -1;

            // If user has tried to input more than 1 character, remove it and set cursor position back
            if (number.Length > 0) number = number.Substring(0, 1);
            if (!int.TryParse(number, out int result)) return -1;
            return result;
        }
    }
}
