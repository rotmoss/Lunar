using System;
using System.Collections.Generic;
using System.Numerics;
using OpenGL;

namespace Lunar.Math
{
    public static partial class FastMath
    {
        public static Vertex2d ToPolar(this Vertex2d v) => new Vertex2d(Cos(v.y * DegreesToRadians) * v.x, Sin(v.y * DegreesToRadians) * v.x);

        public static double Length(this Vertex2d value) => System.Math.Sqrt(value.x * value.x + value.y * value.y);
        public static double Angle(this Vertex2d value) => DiamondAngleToRadians(DiamondAngle(value.x, value.y));
        public static double AngleDegrees(this Vertex2d value) => DiamondAngleToRadians(DiamondAngle(value.x, value.y)) / DegreesToRadians;
        public static Vertex2d Normalize(Vertex2d value) => new Vertex2d(value.x / value.Length(), value.y / value.Length());
        public static Vertex2d Multiply(this Vertex2d a, Vertex2d b) => new Vertex2d(a.x * b.x, a.y * b.y);

        public static Vertex2d Deadzone(this Vertex2d a, double max, double min, double percent)
        {
            percent /= 100f;
            if (a.y < max * percent && a.y > min * percent) a.x = 0;
            if (a.y < max * percent && a.y > min * percent) a.y = 0;
            return a;
        }

        private static double DiamondAngle(double y, double x)
        {
            if (y >= 0)
                return x >= 0 ? y / (x + y) : 1 - x / (-x + y);
            else
                return x < 0 ? 2 - y / (-x - y) : 3 + x / (x - y);
        }

        private static double DiamondAngleToRadians(double dia)
        {
            double x = dia < 2 ? 1 - dia : dia - 3;
            double y = dia < 3 ? ((dia > 1) ? 2 - dia : dia) : dia - 4;

            return System.Math.Atan2(y, x);
        }
    }
}
