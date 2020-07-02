using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Lunar
{
    public struct Transform
    {
        public Vector2 position;
        public Vector2 scale;

        public Transform(float x = 0, float y = 0, float w = 1, float h = 1)
        {
            position = new Vector2(x, y);
            scale = new Vector2(w,h);
        }
        public Transform(Vector2 position, Vector2 scale)
        {
            this.position = position;
            this.scale = scale;
        }

        public static Transform operator +(Transform a, Transform b) => new Transform(a.position + b.position, a.scale * b.scale);
        public static Transform operator +(Transform a, Vector2 b) => new Transform(a.position + b, a.scale);
        public static Transform operator +(Vector2 a, Transform b) => new Transform(b.position + a, b.scale);
    }
}
