using System;

namespace Lunar.Math
{
    public static partial class FastMath
    {
        public static float[] Multiply(this float[] a, float b)
        {
            for (int i = 0; i < a.Length; i++)
            { a[i] *= b; }

            return a;
        }

        public static float Normalize(float x, float min, float max)
        {
            return (x - min) / (max - min);
        }
        public static double Sigmoid(double value, double xMultiple = 1, double yMultiple = 1, double xOffset = 0, double yOffset = 0)
        {
            double k = System.Math.Exp(value * xMultiple) * yMultiple;
            return (k / (1 + xOffset + k)) + yOffset;
        }
    }
}