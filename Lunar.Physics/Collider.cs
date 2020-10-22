using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lunar.Math;
using Lunar.Scenes;
using Lunar.Graphics;

namespace Lunar.Physics
{
    public enum Side
    {
        NONE, TOP, BOTTOM, LEFT, RIGHT
    }

    public class Collider
    {
        private uint id;

        private bool movable;
        private Vector2 offset;
        private Vector2 size;

        private Shape shape;

        public EventHandler<ColissionEventArgs> CollisionEvent { get => _collisionEvent; set => _collisionEvent = value; }
        private EventHandler<ColissionEventArgs> _collisionEvent;

        private static List<Collider> _colliders = new List<Collider>();
        private static Random random = new Random();

        public Collider(uint Id, Vector2 offset, Vector2 size, bool movable = false)
        {
            id = Id;
            this.movable = movable;
            this.offset = offset;
            this.size = size;

            Transform t = new Transform(offset, size);

            shape = new Shape(Id, "vsCollider", _vs, "fsCollider", _fs, 2, true, new float[] { 
                t.position.X - t.scale.X, t.position.Y - t.scale.Y, 
                t.position.X + t.scale.X, t.position.Y - t.scale.Y, 
                t.position.X + t.scale.X, t.position.Y + t.scale.Y, 
                t.position.X - t.scale.X, t.position.Y + t.scale.Y 
            });

            shape.ShaderProgram.SetUniform(new Vector4((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1).ToVertex4f(), "aColor");
            shape.Layer = "Collider";

            _colliders.Add(this);
        }

        public static Vector2[] GetTransforms()
        {
            Vector2[] v = new Vector2[_colliders.Count * 4];

            for (int i = 0, j = 0; i < v.Length; i += 4, j++) {
                Transform t = new Transform(_colliders[j].offset, _colliders[j].size) + Transform.GetGlobalTransform(_colliders[j].id);

                v[i + 0] = new Vector2(t.position.X - t.scale.X, t.position.Y - t.scale.Y);
                v[i + 1] = new Vector2(t.position.X + t.scale.X, t.position.Y - t.scale.Y);
                v[i + 2] = new Vector2(t.position.X + t.scale.X, t.position.Y + t.scale.Y);
                v[i + 3] = new Vector2(t.position.X - t.scale.X, t.position.Y + t.scale.Y);
            }

            return v;
        }

        public static void CheckColissions() => Parallel.ForEach(_colliders, x => x.CheckColission());
        
        public void CheckColission()
        {
            if (!movable) return;

            foreach (Collider collider in _colliders)
            {
                if (IsParent(collider)) continue;

                Transform a = new Transform(offset, size) + Transform.GetGlobalTransform(id);
                Transform b = new Transform(collider.offset, collider.size) + Transform.GetGlobalTransform(collider.id);

                if (DoesOverlap(a, b))
                {
                    Side side = CalculateSide(b.position, b.scale);
                    MoveTransform(side, a, b);
                    OnCollision(new ColissionEventArgs { movable = movable, collider = collider, reciever = this, side = side });
                }
            }
        }

        private bool IsParent(Collider collider)
        {
            if (id == collider.id)
                return true;

            Scene scene = Scene.GetScene(id);
            uint currentId = scene.GetParent(id);

            while (currentId != 0)
                id = scene.GetParent(id);

            return false;
        }

        private Side CalculateSide(Vector2 pos, Vector2 scale)
        {
            //Calulate the angle from the center of b to each of its corners
            float angleTopRight = new Vector2(scale.X, scale.Y).Angle();
            float angleTopLeft = new Vector2(-scale.X, scale.Y).Angle();
            float angleBottomRight = new Vector2(scale.X, -scale.Y).Angle();
            float angleBottomLeft = new Vector2(-scale.X, -scale.Y).Angle();

            float angleBetween = (offset - pos).Angle();

            //Check between which corners a is located

            if (angleBetween < angleBottomRight && angleBetween > angleTopRight)
                return Side.RIGHT;
            if (angleBetween < angleTopLeft && angleBetween > angleBottomLeft)
                return Side.LEFT;
            if (angleBetween < angleTopRight && angleBetween > angleTopLeft)
                return Side.TOP;
            if (angleBetween < angleBottomLeft || angleBetween > angleBottomRight)
                return Side.BOTTOM;

            return Side.NONE;
        }

        private void OnCollision(ColissionEventArgs e)
        {
            EventHandler<ColissionEventArgs> handler = _collisionEvent;
            if (handler == null) { return; }
            handler?.Invoke(null, e);
        }

        private bool DoesOverlap(Transform a, Transform b)
        {
            //If any of the sides from A are outside of B
            if (a.position.Y - a.scale.Y >= b.position.Y + b.scale.Y)
                return false;

            if (a.position.Y + a.scale.Y <= b.position.Y - b.scale.Y)
                return false;

            if (a.position.X + a.scale.X <= b.position.X - b.scale.X)
                return false;

            if (a.position.X - a.scale.X >= b.position.X + b.scale.X)
                return false;

            //If none of the sides from A are outside B
            return true;
        }
        public void MoveTransform(Side side, Transform global, Transform collider)
        {
            float topA, bottomA, leftA, rightA, topB, bottomB, leftB, rightB;

            switch (side)
            {
                case Side.TOP:
                    bottomA = global.position.Y - global.scale.Y;
                    topB = collider.position.Y + collider.scale.Y;

                    Transform.Translate(id, new Vector2(0, topB - bottomA));
                    break;

                case Side.BOTTOM:
                    topA = global.position.Y + global.scale.Y;
                    bottomB = collider.position.Y - collider.scale.Y;

                    Transform.Translate(id, new Vector2(0, bottomB - topA));
                    break;

                case Side.LEFT:
                    rightA = global.position.X + global.scale.X;
                    leftB = collider.position.X - collider.scale.X;

                    Transform.Translate(id, new Vector2(leftB - rightA, 0));
                    break;

                case Side.RIGHT:
                    leftA = global.position.X - global.scale.X;
                    rightB = collider.position.X + collider.scale.X;

                    Transform.Translate(id, new Vector2(rightB - leftA, 0));
                    break; 
            }
        }

        public static void Foreach(Action<Collider> action)
        {
            foreach (Collider collider in _colliders)
                action.Invoke(collider);
        }

        readonly static string[] _vs =
        {
            "#version 330 core",
            "layout(location = 0) in vec2 aPos;",
            "uniform mat4 uProjection;",
            "uniform mat4 uCameraView;",
            "",
            "void main()",
            "{",
            "   gl_Position = uProjection * uCameraView * vec4(aPos, 0.0, 1.0);",
            "}"
        };

        readonly static string[] _fs =
        {
            "#version 330 core",
            "uniform vec4 aColor;",
            "out vec4 FragColor;",
            "void main()",
            "{",
            "   FragColor = aColor;",
            "}",
        };
    }
}