using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public static partial class FastMath
    {
        const float DegreesToRadians = MathF.PI / 180f;

        private static readonly Dictionary<float, float> SineLookup = CreateSineLookup();
        private static readonly float[] SineLookupKeys = SineLookup.Keys.ToArray();

        private static readonly Dictionary<float, float> CosLookup = CreateCosLookup();
        private static readonly float[] CosLookupKeys = CosLookup.Keys.ToArray();

        private static Dictionary<float, float> CreateSineLookup()
        {
            Dictionary<float, float> result = new Dictionary<float, float>();

            for (decimal i = -10; i < 10; i += 0.001m)
                result.Add((float)i, MathF.Sin((float)i));

            return result;
        }

        private static Dictionary<float, float> CreateCosLookup()
        {
            Dictionary<float, float> result = new Dictionary<float, float>();

            for (decimal i = -10; i < 10; i += 0.001m)
                result.Add((float)i, MathF.Cos((float)i));

            return result;
        }

        public static float Sin(float x)
        {
            int index = Array.BinarySearch(SineLookupKeys, x);

            if (index < 0) index = ~index - 1;
            else return MathF.Sin(x);

            try { return SineLookup[SineLookupKeys[index]]; }
            catch { return 0; }
        }

        public static float Cos(float x)
        {
            int index = Array.BinarySearch(CosLookupKeys, x);

            if (index < 0) index = ~index - 1;
            else return MathF.Cos(x);

            try { return CosLookup[CosLookupKeys[index]]; }
            catch { return 0; }
        }
    }
}
