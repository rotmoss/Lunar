using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public struct Vector2D
    {
        const float DegreesToRadians = MathF.PI / 180f;

        public float X
        {
            get
            {
                if (_updateCordinates)
                {
                    _x = FastMath.Cos(_angle) * _length;
                    _y = FastMath.Sin(_angle) * _length;
                    _updateCordinates = false;
                }
                return _x;
            }
        }
        private float _x;
        public float Y {
            get 
            {
                if (_updateCordinates)
                {
                    _x = FastMath.Cos(_angle) * _length;
                    _y = FastMath.Sin(_angle) * _length;
                    _updateCordinates = false;
                }
                return _y;
            } 
        }
        private float _y;

        private bool _updateCordinates;

        public float Angle 
        { 
            get => _angle;
            set { _angle = value; _updateCordinates = true; }
        }
        private float _angle;
        public float Length 
        { 
            get => _length; 
            set { _length = value; _updateCordinates = true; }
        }
        private float _length;

        public Vector2D(float angle = 0, float length = 0, bool convert = true)
        {
            if (convert) _angle = angle * DegreesToRadians;
            else _angle = angle;
            _length = length;
            _x = _y = 0;
            _updateCordinates = true;
        }

        public static Vector2D operator *(Vector2D a, float b) { a.Length *= b;  return a; }
        public static Vector2D operator *(float a, Vector2D b) { b.Length *= a; return b; }

        public static Vector2D operator *(Vector2D a, Vector2D b) 
        {
            a.Length = a.Length * b.Length * (FastMath.Cos(a.Angle - b.Angle)); 
            return b; 
        }

        public static List<Vector2D> Multiply(List<Vector2D> a, float b)
        {
            a = new List<Vector2D>(a);

            if (a.Count < 8 || !Vector.IsHardwareAccelerated)
            {
                for (int i = 0; i < a.Count; i++)
                    a[i] = new Vector2D(a[i].Angle, a[i].Length * b, false);
                return a;
            }

            float[] array = a.Select(x => x.Length).ToArray();
            float[] result = new float[a.Count];
            int left = 0;

            for (int i = 0; i < a.Count - Vector<float>.Count + 1; i += Vector<float>.Count)
            {
                Vector<float> v = new Vector<float>(array, i);
                Vector.Multiply(v, b).CopyTo(result, i);
                left = i;
            }

            for (int i = left + 1; i < a.Count; i++)
                result[i] = array[i] * b;       

            for (int i = 0; i < array.Length; i++)
                a[i] = new Vector2D(a[i].Angle, result[i], false);

            return a;
        }

        public static Transform Sum(List<Vector2D> a)
        {
            Transform value = new Transform(0, 0, 0, 1, 1);
            a.ForEach(x => { value.x += x.X; value.y += x.Y; });
            return value;
        }
    }
}
