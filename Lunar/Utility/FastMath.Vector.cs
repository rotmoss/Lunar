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

        public static List<Vector2> ScaleVectors(List<Vector2> a, float b)
        {
            a = new List<Vector2>(a);

            for (int i = 0; i < a.Count; i++)
                a[i] *= b;
            return a;
        }

        public static List<Vector2> AddVectors(List<Vector2> a, Vector2 b)
        {
            a = new List<Vector2>(a);

            for (int i = 0; i < a.Count; i++)
                a[i] = new Vector2(a[i].X + b.X, a[i].Y + b.Y);
            return a;
        }

        public static float Angle(this Vector2 value)
        {
            return DiamondAngleToRadians(DiamondAngle(value.X, value.Y));
        }

        private static float DiamondAngle(float y, float x)
        {
            if (y >= 0)
                return x >= 0 ? y / (x + y) : 1 - x / (-x + y);
            else
                return x < 0 ? 2 - y / (-x - y) : 3 + x / (x - y);
        }

        private static float DiamondAngleToRadians(float dia)
        {
            float x = dia < 2 ? 1 - dia : dia - 3;
            float y = dia < 3 ? ((dia > 1) ? 2 - dia : dia) : dia - 4;

            return MathF.Atan2(y, x);
        }
    }
}
