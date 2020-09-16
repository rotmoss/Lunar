using System;

namespace Lunar
{
    public static partial class FastMath
    {
        public static float[] Multiply(this float[] a, float b)
        {
            for (int i = 0; i < a.Length; i++)
            { a[i] *= b; }

            return a;
        }

        const int MAX_DELTA_BITS = 1 << 29;
        private static unsafe int FloatToInt32Bits(float f)=> *(int*)&f;
        public static bool AlmostEquals(this float a, float b)
        {
            int aInt = FloatToInt32Bits(a);
            if (aInt < 0) aInt = int.MinValue - aInt;

            int bInt = FloatToInt32Bits(b);
            if (bInt < 0) bInt = int.MinValue - bInt;

            int intDiff = Math.Abs(aInt - bInt);
            return intDiff <= (MAX_DELTA_BITS);
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