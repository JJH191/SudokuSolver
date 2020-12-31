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
    public class BitmapToBitmapImage : IValueConverter
    {
        // TODO: Not my code
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bitmap bitmap = (Bitmap)value;
            if (bitmap == null) return null;

            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height)
            );

            return bitmapSource;
        }

        // Should not need to convert back
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
