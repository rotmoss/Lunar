using System;
using System.Collections.Generic;
using System.Numerics;
using OpenGL;

namespace Lunar.Math
{
    public static partial class FastMath
    {
        public static Vertex2f ToPolar(this Vertex2f v) => new Vertex2f(Cos(v.y * DegreesToRadians) * v.x, Sin(v.y * DegreesToRadians) * v.x);

        public static float Length(this Vertex2f value) => MathF.Sqrt(value.x * value.x + value.y * value.y);
        public static float Angle(this Vertex2f value) => DiamondAngleToRadians(DiamondAngle(value.x, value.y));
        public static float AngleDegrees(this Vertex2f value) => DiamondAngleToRadians(DiamondAngle(value.x, value.y)) / DegreesToRadians;
        public static Vertex2f Normalize(Vertex2f value) => new Vertex2f(value.x / value.Length(), value.y / value.Length());
        public static Vertex2f Multiply(this Vertex2f a, Vertex2f b) => new Vertex2f(a.x * b.x, a.y * b.y);

        public static Vertex2f Deadzone(this Vertex2f a, float max, float min, float percent)
        {
            percent /= 100f;
            if (a.y < max * percent && a.y > min * percent) a.x = 0;
            if (a.y < max * percent && a.y > min * percent) a.y = 0;
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
    }
}
