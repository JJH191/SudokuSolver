using System.Drawing;

namespace DigitClassifier
{
    public interface IDigitClassifier
    {
        int GetDigit(Bitmap bitmap);
    }
}
