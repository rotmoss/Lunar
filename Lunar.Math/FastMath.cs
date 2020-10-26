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
        public static float Sigmoid(float value, float xMultiple = 1, float yMultiple = 1, float xOffset = 0, float yOffset = 0)
        {
            float k = MathF.Exp(value * xMultiple) * yMultiple;
            return (k / (1 + xOffset + k)) + yOffset;
        }
    }
}