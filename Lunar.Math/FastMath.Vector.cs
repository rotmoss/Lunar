using System;
using System.Collections.Generic;
using System.Numerics;
using OpenGL;
using Lunar.Scenes;

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

        public static float Angle(this Vector2 value) => DiamondAngleToRadians(DiamondAngle(value.X, value.Y));
        public static float AngleDegrees(this Vector2 value) => DiamondAngleToRadians(DiamondAngle(value.X, value.Y)) / DegreesToRadians;    

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

        public static Vertex2f ToVertex4f(this Vector2 vec) => new Vertex2f(vec.X, vec.Y);
        public static Vertex4f ToVertex3f(this Vector3 vec) => new Vertex3f(vec.X, vec.Y, vec.Z);
        public static Vertex4f ToVertex4f(this Vector4 vec) => new Vertex4f(vec.X, vec.Y, vec.Z, vec.W);
        public static Vertex2f[] ToVertex2fArray(this Vector2[] data)
        {
            Vertex2f[] v = new Vertex2f[data.Length];

            for (int i = 0; i < data.Length; i++)
                v[i] = new Vertex2f(data[i].X, data[i].Y);
            return v;
        }
        public static Vertex3f[] ToVertex3fArray(this Vector3[] data)
        {
            Vertex3f[] v = new Vertex3f[data.Length];

            for (int i = 0; i < data.Length; i++)
                v[i] = new Vertex3f(data[i].X, data[i].Y, data[i].Z);
            return v;
        }
        public static Vertex4f[] ToVertex4fArray(this Vector4[] data)
        {
            Vertex4f[] v = new Vertex4f[data.Length];

            for (int i = 0; i < data.Length; i++)
                v[i] = new Vertex4f(data[i].X, data[i].Y, data[i].Z, data[i].W);
            return v;
        }

        public static Matrix4x4f ToMatrix4x4f(this Transform t) => new Matrix4x4f(
            t.scale.X, 0, 0, 0,
            0, t.scale.Y, 0, 0,
            0, 0, 0, 0,
            t.position.X, t.position.Y, 0, 1
        );
    }
}
