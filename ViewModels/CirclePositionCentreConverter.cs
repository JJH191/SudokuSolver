using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ViewModels
{
    /// <summary>
    /// Converts the value given so that the centre of the circle is aligned to it
    /// </summary>
    public class CirclePositionCentreConverter : IValueConverter
    {
        private readonly double circleRadius;

        public CirclePositionCentreConverter(double circleRadius)
        {
            this.circleRadius = circleRadius;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value - circleRadius;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
