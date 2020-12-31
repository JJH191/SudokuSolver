using System;
using System.Globalization;
using System.Windows.Data;

namespace ViewModels.Converters
{
    /// <summary>
    /// Converts the value given so that the centre of the circle is aligned to it
    /// </summary>
    public class CirclePositionCentreConverter : IValueConverter
    {
        // Radius of the circle being drawn
        private readonly double circleRadius;

        public CirclePositionCentreConverter(double circleRadius)
        {
            this.circleRadius = circleRadius;
        }

        // Centre the circle by subtracting the radius (half the width) from the x and y coordinates
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value - circleRadius;
        }

        // Should not need to convert back from the circle's position
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
