using System;
using System.Collections.Generic;
using OpenGL;
using Lunar.Math;

namespace Lunar.Scenes
{
    public class Transform
    {
        public static Transform Zero = new Transform(0, 0, 1, 1);

        public Vertex2f position;
        public Vertex2f scale;

        private static Dictionary<uint, Transform> _transforms = new Dictionary<uint, Transform>();

        public Transform(float x = 0, float y = 0, float w = 1, float h = 1)
        {
            position = new Vertex2f(x, y);
            scale = new Vertex2f(w, h);
        }
        public Transform(Transform transform)
        {
            position = transform.position;
            scale = transform.scale;
        }
        public Transform(Vertex2f position, Vertex2f scale)
        {
            this.position = position;
            this.scale = scale;
        }

        public static Transform operator +(Transform a, Transform b) => new Transform(a.position + b.position, a.scale.Multiply(b.scale));
        public static Transform operator +(Transform a, Vertex2f b) => new Transform(a.position + b, a.scale);
        public static Vertex2f operator +(Vertex2f a, Transform b) => new Vertex2f(b.position.x + a.x, b.position.y + a.y);

        public static Transform operator -(Transform a, Transform b) => new Transform(a.position - b.position, a.scale);
        public static Transform operator -(Transform a, Vertex2f b) => new Transform(a.position - b, a.scale);
        public static Vertex2f operator -(Vertex2f a, Transform b) => new Vertex2f(b.position.x - a.x, b.position.y - a.y);
        public static Transform operator -(Transform a) => new Transform(-a.position, a.scale);

        public static Transform operator *(Transform a, Vertex2f b) => new Transform(a.position.x, a.position.y, a.scale.x * b.x, a.scale.y * b.y);
        public static Vertex2f operator *(Vertex2f a, Transform b) => new Vertex2f(b.scale.x * a.x, b.scale.y * a.y);
        public static Transform operator *(Transform a, float b) => new Transform(a.position.x, a.position.y, a.scale.x * b, a.scale.y * b);

        public static Transform operator /(Transform a, Vertex2f b) => new Transform(a.position.x, a.position.y, a.scale.x / b.x, a.scale.y / b.y);
        public static Vertex2f operator /(Vertex2f a, Transform b) => new Vertex2f(a.x / b.scale.x, a.y / b.scale.y);
        public static Transform operator /(Transform a, float b) => new Transform(a.position.x, a.position.y, a.scale.x / b, a.scale.y / b);
        public static void SetTransform(uint id, Transform value) { if (_transforms.ContainsKey(id)) _transforms[id] = value; }
        public static void Translate(uint id, Vertex2f value) { if (_transforms.ContainsKey(id)) _transforms[id] += value; }
        public static void Scale(uint id, Vertex2f value) { if (_transforms.ContainsKey(id)) _transforms[id] *= value; }
        public static void Scale(uint id, float value) { if (_transforms.ContainsKey(id)) _transforms[id] *= value; }
        public Matrix4x4f ToMatrix4x4f() => new Matrix4x4f(
            (float)scale.x, 0, 0, 0,
            0, MathF.Floor((float)scale.y), 0, 0,
            0, 0, 0, 0,
            (float)position.x, (float)position.y, 0, 1
        );
        public static Transform GetLocalTransform(uint id) => _transforms.ContainsKey(id) ? _transforms[id] : Zero;
        public static Transform GetGlobalTransform(uint id)
        {
            Scene scene = Scene.GetScene(id);
            if (scene == null) 
                return Zero;

            Transform t = Zero;
            do
            {
                t += _transforms.ContainsKey(id) ? _transforms[id] : Zero;
                id = scene.GetParent(id);
            } while (id != 0);
            return t;
        }
        public static void AddTransform(uint id)
        {
            if (!_transforms.ContainsKey(id))
            {
                _transforms.Add(id, new Transform());
            }
        }
    }
}
