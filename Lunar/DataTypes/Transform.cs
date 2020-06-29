using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Lunar
{
    public struct Transform
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public float h;

        public Transform(float x = 0, float y = 0, float z = 0, float w = 1, float h = 1)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            this.h = h;
        }

        public static Transform operator +(Transform a, Transform b) => new Transform(a.x + b.x, a.y + b.y, a.z + b.z, a.w * b.w, a.h * b.h);
        public static Transform operator +(Transform a, Vector2D b) { a.x += b.X; a.y += b.Y; return a; }
    }
}
