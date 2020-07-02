using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lunar
{
    static partial class FastMath
    {
        public static Vector2 SumVectors(List<Vector2> vectors)
        {
            Vector2 value = new Vector2(0, 0);
            for (int i = 0; i < vectors.Count; i++)
                value += vectors[i];
            return value;
        }

        public static List<Vector2> MultiplyVectors(List<Vector2> a, float b)
        {
            a = new List<Vector2>(a);

            for (int i = 0; i < a.Count; i++)
                a[i] *= b;
            return a;
        }
    }
}
