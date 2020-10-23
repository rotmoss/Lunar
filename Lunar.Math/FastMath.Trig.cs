using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.Math
{
    public static partial class FastMath
    {
        const double DegreesToRadians = System.Math.PI / 180d;

        private static readonly Dictionary<double, double> SineLookup = CreateSineLookup();
        private static readonly double[] SineLookupKeys = SineLookup.Keys.ToArray();

        private static readonly Dictionary<double, double> CosLookup = CreateCosLookup();
        private static readonly double[] CosLookupKeys = CosLookup.Keys.ToArray();

        private static Dictionary<double, double> CreateSineLookup()
        {
            Dictionary<double, double> result = new Dictionary<double, double>();

            for (decimal i = -10; i < 10; i += 0.001m)
                result.Add((double)i, System.Math.Sin((double)i));

            return result;
        }

        private static Dictionary<double, double> CreateCosLookup()
        {
            Dictionary<double, double> result = new Dictionary<double, double>();

            for (decimal i = -10; i < 10; i += 0.001m)
                result.Add((double)i, System.Math.Cos((double)i));

            return result;
        }

        public static double Sin(double x)
        {
            int index = Array.BinarySearch(SineLookupKeys, x);

            if (index < 0) index = ~index - 1;
            else return System.Math.Sin(x);

            try { return SineLookup[SineLookupKeys[index]]; }
            catch { return 0; }
        }

        public static double Cos(double x)
        {
            int index = Array.BinarySearch(CosLookupKeys, x);

            if (index < 0) index = ~index - 1;
            else return System.Math.Cos(x);

            try { return CosLookup[CosLookupKeys[index]]; }
            catch { return 0; }
        }
    }
}
