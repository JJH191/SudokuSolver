using System;

namespace Common
{
    /// <summary>
    /// A simple ascii progress bar for the console
    /// </summary>
    public class ProgressBar
    {
        // Characters to build the progress bar from
        private static readonly char fillChar = '█';
        private static readonly char emptyChar = ' ';
        private static readonly char startChar = '|';
        private static readonly char endChar = startChar;

        // Background colour for the progress bar
        private static readonly ConsoleColor backgroundColour = ConsoleColor.DarkGray;

        private readonly float max; // Maximum value that can be reached
        private readonly int length; // Length of the bar in the console
        private readonly bool printPercentage; // Whether the percentage should be printed after the bar

        // Keeps track of whether it is the first print so that the cursor is only reset on subsequent calls of PrintProgress
        private bool isFirstPrint = true;

        /// <summary>
        /// Create a new progress bar
        /// </summary>
        /// <param name="max">The maximum value to be reached</param>
        /// <param name="length">The length of the bar in the console</param>
        /// <param name="printPercentage">Whether the percentage should be printed after the progress bar</param>
        public ProgressBar(float max, int length, bool printPercentage = true)
        {
            this.max = max;
            this.length = length;
            this.printPercentage = printPercentage;
        }

        /// <summary>
        /// Prints the progress bar - this should be called in a loop to update the bar
        /// Nothing should be printed between calling this function, or it will not work
        /// </summary>
        /// <param name="current">The current value used to calculate the progress</param>
        public void PrintProgress(float current)
        {
            // Calculate the percentage of the maximum value
            float percentage = current / max;

            // Get the string for the progress bar given the percentage
            string progressBar = GetProgressString(percentage);

            // Only reset the cursor AFTER the first print
            if (!isFirstPrint) Console.SetCursorPosition(0, Console.CursorTop - 1);
            else isFirstPrint = false;

            // Print the bar, changing the background colour for the filled and empty areas
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(startChar);

            Console.BackgroundColor = backgroundColour;
            Console.Write(progressBar);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(endChar);

            // Print the percentage at the end if specified to do so
            if (printPercentage) Console.Write($" {string.Format("{0:0.00}", percentage * 100)}%");
            Console.WriteLine();
        }


        /// <summary>
        /// Works out the bar string given a percentage
        /// </summary>
        /// <param name="percentage">Percentage of the bar to fill</param>
        /// <returns>Ascii representation of the percentage bar</returns>
        public string GetProgressString(float percentage)
        {
            string bar = "";

            // Loop through the length of the string and if the percentage along the bar is less than the current percentage, use the fill character, otherwise the empty character
            for (int i = 0; i < length; i++)
            {
                if ((float)i / length < percentage) bar += fillChar;
                else bar += emptyChar;
            }

            return bar;
        }
    }
}
