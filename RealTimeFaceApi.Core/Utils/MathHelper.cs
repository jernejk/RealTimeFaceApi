using System;
using System.Drawing;

namespace RealTimeFaceApi.Core.Utils
{
    public static class MathHelper
    {
        public static double GetDistancePow2(Point a, Point b)
        {
            return Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);
        }
    }
}
