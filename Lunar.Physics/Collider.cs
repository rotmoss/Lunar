using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lunar.Math;
using Lunar.Scenes;
using Lunar.Graphics;
using OpenGL;

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
        private Vertex2f offset;
        private Vertex2f size;

        private Polygon shape;

        public EventHandler<ColissionEventArgs> CollisionEvent { get => _collisionEvent; set => _collisionEvent = value; }
        private EventHandler<ColissionEventArgs> _collisionEvent;

        private static List<Collider> _colliders = new List<Collider>();
        private static Random random = new Random();
        public static bool DrawColliders { get => _drawColliders; set => _drawColliders = value; }
        private static bool _drawColliders = false;

        public Collider(uint Id, Vertex2f offset, Vertex2f size, bool movable = false)
        {
            id = Id;
            this.movable = movable;
            this.offset = offset;
            this.size = size;

            if (_drawColliders)
            {
                Transform t = new Transform(offset, size);

                shape = new Polygon(Id, "Collider.vert", "Collider.frag", 2, true, new float[] {
                t.position.x - t.scale.x, t.position.y - t.scale.y,
                t.position.x + t.scale.x, t.position.y - t.scale.y,
                t.position.x + t.scale.x, t.position.y + t.scale.y,
                t.position.x - t.scale.x, t.position.y + t.scale.y
                });

                shape.ShaderProgram.SetUniform((Vertex4f)new Vertex4d(random.NextDouble(), random.NextDouble(), random.NextDouble(), 1), "aColor");
                shape.Layer = "Collider";
            }

            _colliders.Add(this);
        }

        public static Vertex2f[] GetTransforms()
        {
            Vertex2f[] v = new Vertex2f[_colliders.Count * 4];

            for (int i = 0, j = 0; i < v.Length; i += 4, j++) {
                Transform t = new Transform(_colliders[j].offset, _colliders[j].size) + Transform.GetGlobalTransform(_colliders[j].id);

                v[i + 0] = new Vertex2f(t.position.x - t.scale.x, t.position.y - t.scale.y);
                v[i + 1] = new Vertex2f(t.position.x + t.scale.x, t.position.y - t.scale.y);
                v[i + 2] = new Vertex2f(t.position.x + t.scale.x, t.position.y + t.scale.y);
                v[i + 3] = new Vertex2f(t.position.x - t.scale.x, t.position.y + t.scale.y);
            }

            return v;
        }

        public static void CheckColissions() { foreach(Collider x in _colliders) x.CheckColission(); }
        
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

        private Side CalculateSide(Vertex2f pos, Vertex2f scale)
        {
            //Calulate the angle from the center of b to each of its corners
            float angleTopRight = new Vertex2f(scale.x, scale.y).Angle();
            float angleTopLeft = new Vertex2f(-scale.x, scale.y).Angle();
            float angleBottomRight = new Vertex2f(scale.x, -scale.y).Angle();
            float angleBottomLeft = new Vertex2f(-scale.x, -scale.y).Angle();

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
            if (a.position.y - a.scale.y >= b.position.y + b.scale.y)
                return false;

            if (a.position.y + a.scale.y <= b.position.y - b.scale.y)
                return false;

            if (a.position.x + a.scale.x <= b.position.x - b.scale.x)
                return false;

            if (a.position.x - a.scale.x >= b.position.x + b.scale.x)
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
                    bottomA = global.position.y - global.scale.y;
                    topB = collider.position.y + collider.scale.y;

                    Transform.Translate(id, new Vertex2f(0, topB - bottomA));
                    break;

                case Side.BOTTOM:
                    topA = global.position.y + global.scale.y;
                    bottomB = collider.position.y - collider.scale.y;

                    Transform.Translate(id, new Vertex2f(0, bottomB - topA));
                    break;

                case Side.LEFT:
                    rightA = global.position.x + global.scale.x;
                    leftB = collider.position.x - collider.scale.x;

                    Transform.Translate(id, new Vertex2f(leftB - rightA, 0));
                    break;

                case Side.RIGHT:
                    leftA = global.position.x - global.scale.x;
                    rightB = collider.position.x + collider.scale.x;

                    Transform.Translate(id, new Vertex2f(rightB - leftA, 0));
                    break; 
            }
        }

        public static void Foreach(Action<Collider> action)
        {
            foreach (Collider collider in _colliders)
                action.Invoke(collider);
        }
    }
}