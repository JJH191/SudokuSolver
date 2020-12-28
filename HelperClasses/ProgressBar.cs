using System;

namespace TrainingInterface
{
    public class ProgressBar
    {
        private static readonly char fillChar = '█';
        private static readonly char emptyChar = '-';
        private static readonly char startChar = '[';
        private static readonly char endChar = ']';

        private readonly float max;
        private readonly int length;
        private bool isFirstPrint = true;
        private bool printPercentage;

        public ProgressBar(float max, int length, bool printPercentage = true)
        {
            this.max = max;
            this.length = length;
            this.printPercentage = printPercentage;
        }

        public void PrintProgress(float current)
        {
            float percentage = current / max;

            string progressBar = GetProgressString(percentage);
            if (!isFirstPrint)
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            else
                isFirstPrint = false;
            Console.WriteLine(progressBar);
        }

        public string GetProgressString(float percentage)
        {
            string bar = startChar.ToString();

            for (int i = 0; i < length; i++)
            {
                if ((float)i / length < percentage) bar += fillChar;
                else bar += emptyChar;
            }

            bar += endChar;
            if (printPercentage) bar += $" {string.Format("{0:0.00}", percentage * 100)}%";
            return bar;
        }
    }
}
