﻿using OpenGL;

namespace Lunar
{
    public class Transform : UniqueComponent<Transform>
    {
        public static Transform Zero = new Transform(0, 0, 1, 1);

        public Vertex2f position;
        public Vertex2f scale;

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

        public static Transform operator +(Transform a, Transform b) => new Transform(a.position + b.position, new Vertex2f(a.scale.x * b.scale.y, b.scale.x * b.scale.y));
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

        public static void MoveTransform(uint id, Vertex2f position) { if (_components.ContainsKey(id)) { _components[id] += position; } }
        public static void ScaleTransform(uint id, Vertex2f scale) { if (_components.ContainsKey(id)) { _components[id] *= scale; } }
        public static void SetTransform(uint id, Transform transform) { if (_components.ContainsKey(id)) { _components[id] = transform; } }

        public static Transform GetLocalTransform(uint id) 
        {
            return _components.ContainsKey(id) ? _components[id] : Zero; 
        }

        public static Transform GetGlobalTransform(uint id)
        {
            Transform t = _components.ContainsKey(id) ? _components[id] : Zero;
            foreach (uint parent in Gameobject.GetParents(id))
                t += _components.ContainsKey(id) ? _components[id] : Zero;

            return t;
        }

        public override void DisposeChild()
        {
            throw new System.NotImplementedException();
        }

        public Matrix4x4f ToMatrix4x4f() => new Matrix4x4f(
            scale.x, 0, 0, 0,
            0, scale.y, 0, 0,
            0, 0, 0, 0,
            position.x, position.y, 0, 1
        );
    }
}
