using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public struct Vector
    {
        public float X { get => _x; }
        private float _x;
        public float Y { get => _y; }
        private float _y;

        public float Angle { get => _angle; set => _angle = value; }
        private float _angle;
        public float Length { get => _length; set => _length = value; }
        private float _length;

        public Vector(float angle = 0, float length = 0)
        {
            _angle = angle;
            _length = length;

            _x = MathF.Cos(_angle * (MathF.PI / 180f)) * _length;
            _y = MathF.Sin(_angle * (MathF.PI / 180f)) * _length;
        }

        public static Vector operator *(Vector a, float b) { return new Vector(a._angle, a._length * b); }
        public static Vector operator *(float a, Vector b) { return new Vector(b._angle, b._length * a); }
    }
}
