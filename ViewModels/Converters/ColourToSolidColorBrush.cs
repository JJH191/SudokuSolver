using Common;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ViewModels.Converters
{
    /// <summary>
    /// Converts a given colour to a SolidColourBrush to be used for colouring UI
    /// </summary>
    public class ColourToSolidColourBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Colour colour = (Colour)value;
            if (colour == null) return null;

            return new SolidColorBrush(colour);
        }

        // Should not need to convert back
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
