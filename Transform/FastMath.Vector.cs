using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lunar.Math
{
    public static partial class FastMath
    {
        public static Vector2 ToPolar(this Vector2 v) => new Vector2(Cos(v.Y * DegreesToRadians) * v.X, Sin(v.Y * DegreesToRadians) * v.X);

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

        public static Vector2 Deadzone(this Vector2 a, float max, float min, float percent)
        {
            percent /= 100f;
            if (a.X < max * percent && a.X > min * percent) a.X = 0;
            if (a.Y < max * percent && a.Y > min * percent) a.Y = 0;
            return a;
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

        public static float[] ToFloatArray(this Vector2[] array)
        {
            float[] temp = new float[array.Length * 2];

            for (int i = 0, j = 0; i < array.Length; i++, j += 2)
            {
                temp[j] = array[i].X;
                temp[j + 1] = array[i].Y;
            }

            return temp;
        }
        public static float[] ToFloatArray(this Vector3[] array)
        {
            float[] temp = new float[array.Length * 3];

            for (int i = 0, j = 0; i < array.Length; i++, j += 3)
            {
                temp[j] = array[i].X;
                temp[j + 1] = array[i].Y;
                temp[j + 2] = array[i].Z;
            }

            return temp;
        }
    }
}
