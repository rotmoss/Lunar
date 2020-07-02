using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    static partial class FastMath
    {
        public static void Init()
        {
            float i = SineLookup[SineLookupKeys[0]];
            i = CosLookup[CosLookupKeys[0]];
        }

        public static float[] Multiply(this float[] a, float b)
        {
            for (int i = 0; i < a.Length; i++)
            { a[i] *= b; }

            return a;
        }

        private static readonly float[] RoundLookup = CreateRoundLookup();

        private static float[] CreateRoundLookup()
        {
            float[] result = new float[15];

            for (int i = 0; i < result.Length; i++)
                result[i] = MathF.Pow(10, i);

            return result;
        }

        public static float Round(float value, int decimalPlaces)
        {
            float adjustment = RoundLookup[decimalPlaces];
            return MathF.Floor(value * adjustment + 0.5f) / adjustment;
        }

        public static float Round(float value)
        {
            return MathF.Floor(value + 0.5f);
        }

        const int MAX_DELTA_BITS = 1 << 29;
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
