namespace HelperClasses
{
    public class Colour
    {
        public int A { get; }
        public int R { get; }
        public int G { get; }
        public int B { get; }

        public Colour(int a, int r, int g, int b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public Colour(int r, int g, int b)
        {
            A = 255;
            R = r;
            G = g;
            B = b;
        }

        public Colour(System.Drawing.Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public Colour(System.Windows.Media.Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public float GetBrightness()
        {
            return (0.2126f * R + 0.7152f * G + 0.0722f * B) / 255;
        }

        public static implicit operator Colour(System.Drawing.Color color) => new Colour(color);
        public static implicit operator Colour(System.Windows.Media.Color color) => new Colour(color);
        public static implicit operator System.Drawing.Color(Colour color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        public static implicit operator System.Windows.Media.Color(Colour color) => System.Windows.Media.Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
    }
}
