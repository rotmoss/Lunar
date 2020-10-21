using System.Numerics;
using System.Collections.Generic;

namespace Lunar.Scenes
{
    public class Transform
    {
        public static Transform Zero = new Transform(0, 0, 1, 1);

        public Vector2 position;
        public Vector2 scale;

        private static Dictionary<uint, Transform> _transforms = new Dictionary<uint, Transform>();

        public Transform(float x = 0, float y = 0, float w = 1, float h = 1)
        {
            position = new Vector2(x, y);
            scale = new Vector2(w, h);
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

        public static Transform operator +(Transform a, Transform b) => new Transform(a.position + b.position, a.scale * b.scale);
        public static Transform operator +(Transform a, Vector2 b) => new Transform(a.position + b, a.scale);
        public static Vector2 operator +(Vector2 a, Transform b) => new Vector2(b.position.X + a.X, b.position.Y + a.Y);

        public static Transform operator -(Transform a, Transform b) => new Transform(a.position - b.position, a.scale);
        public static Transform operator -(Transform a, Vector2 b) => new Transform(a.position - b, a.scale);
        public static Vector2 operator -(Vector2 a, Transform b) => new Vector2(b.position.X - a.X, b.position.Y - a.Y);
        public static Transform operator -(Transform a) => new Transform(-a.position, a.scale);

        public static Transform operator *(Transform a, Vector2 b) => new Transform(a.position.X, a.position.Y, a.scale.X * b.X, a.scale.Y * b.Y);
        public static Vector2 operator *(Vector2 a, Transform b) => new Vector2(b.scale.X * a.X, b.scale.Y * a.Y);
        public static Transform operator *(Transform a, float b) => new Transform(a.position.X, a.position.Y, a.scale.X * b, a.scale.Y * b);

        public static Transform operator /(Transform a, Vector2 b) => new Transform(a.position.X, a.position.Y, a.scale.X / b.X, a.scale.Y / b.Y);
        public static Vector2 operator /(Vector2 a, Transform b) => new Vector2(a.X / b.scale.X, a.Y / b.scale.Y);
        public static Transform operator /(Transform a, float b) => new Transform(a.position.X, a.position.Y, a.scale.X / b, a.scale.Y / b);
        public static void SetTransform(uint id, Transform value) { if (_transforms.ContainsKey(id)) _transforms[id] = value; }
        public static void Translate(uint id, Vector2 value) { if (_transforms.ContainsKey(id)) _transforms[id] += value; }
        public static void Scale(uint id, Vector2 value) { if (_transforms.ContainsKey(id)) _transforms[id] *= value; }
        public static void Scale(uint id, float value) { if (_transforms.ContainsKey(id)) _transforms[id] *= value; }

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
