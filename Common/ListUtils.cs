using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ListUtils
    {
        private static readonly Random rng = new Random();

        // Code from https://stackoverflow.com/questions/273313/randomize-a-listt
        // Modified to work for arrays instead of lists
        public static void Shuffle<T>(this T[] list)
        {
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
