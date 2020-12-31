namespace HelperClasses
{
    /// <summary>
    /// Colour wrapper for System.Windows.Media.Color and System.Drawing.Color
    /// Allows implicit conversion between these two classes
    /// </summary>
    public class Colour
    {
        // Alpha, Red, Green, Blue
        public int A { get; }
        public int R { get; }
        public int G { get; }
        public int B { get; }

        /// <summary>
        /// Create a colour with alpha <paramref name="a"/>, red <paramref name="r"/>, green <paramref name="g"/> and blue <paramref name="b"/>
        /// </summary>
        /// <param name="a">Alpha value</param>
        /// <param name="r">Red value</param>
        /// <param name="g">Green value</param>
        /// <param name="b">Blue value</param>
        public Colour(int a, int r, int g, int b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Create a colour with red <paramref name="r"/>, green <paramref name="g"/> and blue <paramref name="b"/>
        /// Alpha is set to 255 (maximum)
        /// </summary>
        /// <param name="r">Red value</param>
        /// <param name="g">Green value</param>
        /// <param name="b">Blue value</param>
        public Colour(int r, int g, int b)
        {
            A = 255;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Create a colour with the ARGB values from System.Drawing.Color
        /// </summary>
        /// <param name="color">The color to convert</param>
        public Colour(System.Drawing.Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        /// <summary>
        /// Create a colour with the ARGB values from System.Windows.Media.Color
        /// </summary>
        /// <param name="color">The color to convert</param>
        public Colour(System.Windows.Media.Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        /// <summary>
        /// Gets the brightness of the colour
        /// </summary>
        /// <returns>The colour brightness</returns>
        public float GetBrightness()
        {
            return (0.2126f * R + 0.7152f * G + 0.0722f * B) / 255;
        }

        // Implicit conversion between Colour, System.Windows.Media.Color and System.Drawing.Color
        public static implicit operator Colour(System.Drawing.Color color) => new Colour(color);
        public static implicit operator Colour(System.Windows.Media.Color color) => new Colour(color);
        public static implicit operator System.Drawing.Color(Colour color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        public static implicit operator System.Windows.Media.Color(Colour color) => System.Windows.Media.Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
    }
}
