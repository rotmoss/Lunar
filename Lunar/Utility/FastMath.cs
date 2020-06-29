using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    static class FastMath
    {
        const int MAX_DELTA_BITS = 1 << 29;
        const float TAU = MathF.PI / 2f;

        private static readonly float[] RoundLookup = CreateRoundLookup();

        private static float[] CreateRoundLookup()
        {
            float[] result = new float[15];

            for (int i = 0; i < result.Length; i++)
                result[i] = MathF.Pow(10, i);

            return result;
        }

        private static readonly Dictionary<float, float> SineLookup = CreateSineLookup();
        private static readonly float[] SineLookUpKeys = SineLookup.Keys.ToArray();

        private static Dictionary<float, float> CreateSineLookup()
        {
            Dictionary<float, float> result = new Dictionary<float, float>();

            for (decimal i = -10; i < 10; i += 0.001m)
                result.Add((float)i, MathF.Sin((float)i));
            
            return result;
        }

        public static void Init()
        {
            float i = SineLookup[SineLookUpKeys[0]];
        }

        public static float Sin(float x)
        {
            int index = Array.BinarySearch(SineLookUpKeys, x);

            if (index < 0) index = ~index - 1;
            else return MathF.Sin(x);

            try { return SineLookup[SineLookUpKeys[index]]; }
            catch { return 0; }
        }

        public static float Cos(float x)
        {
            int index = Array.BinarySearch(SineLookUpKeys, x + TAU);

            if (index < 0) index = ~index - 1;
            else return MathF.Cos(x);

            try { return SineLookup[SineLookUpKeys[index]]; }
            catch { return 0; }
        }

        public static float[] Multiply(this float[] a, float b)
        {
            for (int i = 0; i < a.Length; i++)
            { a[i] *= b; }

            return a;
        }

        public static float Round(float value)
        {
            return MathF.Floor(value + 0.5f);
        }

        public static float Round(float value, int decimalPlaces)
        {
            float adjustment = RoundLookup[decimalPlaces];
            return MathF.Floor(value * adjustment + 0.5f) / adjustment;
        }

        public static bool AlmostEquals(this float a, float b)
        {
            int aInt = FloatToInt32Bits(a);
            if (aInt < 0) aInt = int.MinValue - aInt;

            int bInt = FloatToInt32Bits(b);
            if (bInt < 0) bInt = int.MinValue - bInt;

            int intDiff = Math.Abs(aInt - bInt);
            return intDiff <= (MAX_DELTA_BITS);
        }

        private static unsafe int FloatToInt32Bits(float f)
        {
            return *((int*)&f);
        }
    }
}
