﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Utils
    {
        // Algorithm from https://stackoverflow.com/questions/345187/math-mapping-numbers
        public static double MapRange(double value, double min1, double max1, double min2, double max2)
        {
            return (value - min1) / (max1 - min1) * (max2 - min2) + min2;
        }
    }
}
