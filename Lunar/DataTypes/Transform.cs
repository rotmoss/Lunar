using System.Numerics;

namespace Lunar
{
    public struct Transform
    {
        public static Transform Zero = new Transform(0, 0, 1, 1);

        public Vector2 position;
        public Vector2 scale;

        public Transform(float x = 0, float y = 0, float w = 1, float h = 1)
        {
            position = new Vector2(x, y);
            scale = new Vector2(w,h);
        }
        public Transform(Transform transform)
        {
            position = transform.position;
            scale = transform.scale;
        }
        public Transform(Vector2 position, Vector2 scale)
        {
            this.position = position;
            this.scale = scale;
        }

        public OpenGL.Matrix4x4f ToMatrix4x4f() => new OpenGL.Matrix4x4f(
            scale.X, 0, 0, 0, 
            0, scale.Y, 0, 0, 
            0, 0, 0, 0, 
            position.X, position.Y, 0, 1
        ); 

        public static Transform operator +(Transform a, Transform b) => new Transform(a.position + b.position, a.scale);
        public static Transform operator +(Transform a, Vector2 b) => new Transform(a.position + b, a.scale);
        public static Vector2 operator +(Vector2 a, Transform b) => new Vector2(b.position.X + a.X, b.position.Y + a.Y);

        public static Transform operator -(Transform a) => new Transform(-a.position, a.scale);
        public static Transform operator -(Transform a, Transform b) => new Transform(a.position - b.position, a.scale);
        public static Transform operator -(Transform a, Vector2 b) => new Transform(a.position - b, a.scale);
        public static Vector2 operator -(Vector2 a, Transform b) => new Vector2(b.position.X - a.X, b.position.Y - a.Y);

        public static Transform operator *(Transform a, Vector2 b) => new Transform(a.position.X, a.position.Y, a.scale.X * b.X, a.scale.Y * b.Y);
        public static Vector2 operator *(Vector2 a, Transform b) => new Vector2(b.scale.X * a.X, b.scale.Y * a.Y);
        public static Transform operator *(Transform a, float b) => new Transform(a.position.X, a.position.Y, a.scale.X * b, a.scale.Y * b);

        public static Transform operator /(Transform a, Vector2 b) => new Transform(a.position.X, a.position.Y, a.scale.X / b.X, a.scale.Y / b.Y);
        public static Vector2 operator /(Vector2 a, Transform b) => new Vector2(a.X / b.scale.X, a.Y / b.scale.Y);
        public static Transform operator /(Transform a, float b) => new Transform(a.position.X, a.position.Y, a.scale.X / b, a.scale.Y / b);

        public void Translate(Vector2 position) { this.position.X += position.X; this.position.Y += position.Y; }
        public void Scale(Vector2 scale) { this.scale.X += scale.X; this.scale.Y += scale.Y; }
        public void Translate(float x, float y) { position.X += x; position.Y += y; }
        public void Scale(float x, float y) { scale.X += x; scale.Y += y; }
    }
}
