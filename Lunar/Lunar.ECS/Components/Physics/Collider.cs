using System;
using System.Collections.Generic;
using Lunar.Math;
using OpenGL;

namespace Lunar.ECS.Components
{
    public enum Side
    {
        NONE, TOP, BOTTOM, LEFT, RIGHT
    }

    public class Collider : ObjectIdentifier<Collider>, IObjectIdentifier
    {
        private bool _movable;
        private Vertex2f _offset;
        private Vertex2f _size;

        private Polygon _shape;

        public EventHandler<ColissionEventArgs> CollisionEvent { get => _collisionEvent; set => _collisionEvent = value; }
        private EventHandler<ColissionEventArgs> _collisionEvent;

        private static List<Collider> _colliders = new List<Collider>();
        private static Random random = new Random();

        public static bool DrawColliders { get => _drawColliders; set => _drawColliders = value; }
        private static bool _drawColliders = false;

        public Collider(Vertex2f offset, Vertex2f size, bool movable = false) : base()
        {
            _movable = movable;
            _offset = offset;
            _size = size;

            if (_drawColliders)
            {
                Transform t = new Transform(offset, size);

                _shape = new Polygon("Collider.vert", "Collider.frag", 2, true, new float[] {
                t.position.x - t.scale.x, t.position.y - t.scale.y,
                t.position.x + t.scale.x, t.position.y - t.scale.y,
                t.position.x + t.scale.x, t.position.y + t.scale.y,
                t.position.x - t.scale.x, t.position.y + t.scale.y
                });

                _shape.ShaderProgram.SetUniform((Vertex4f)new Vertex4d(random.NextDouble(), random.NextDouble(), random.NextDouble(), 1), "aColor");
                _shape.Layer = "Collider";
            }

            _colliders.Add(this);
        }
        
        public void CheckColission()
        {
            if (!_movable) return;

            foreach (Collider collider in _colliders)
            {
                if (Gameobject.HierarchyTree.IsParent(Id, collider.Id)) continue;

                Transform a = new Transform(_offset, _size) + Transform.GetGlobalTransform(Id);
                Transform b = new Transform(collider._offset, collider._size) + Transform.GetGlobalTransform(collider.Id);

                if (DoesOverlap(a, b))
                {
                    Side side = CalculateSide(b.position, b.scale);
                    MoveTransform(side, a, b);
                    OnCollision(new ColissionEventArgs { movable = _movable, collider = collider, reciever = this, side = side });
                }
            }
        }

        private Side CalculateSide(Vertex2f pos, Vertex2f scale)
        {
            //Calulate the angle from the center of b to each of its corners
            float angleTopRight = new Vertex2f(scale.x, scale.y).Angle();
            float angleTopLeft = new Vertex2f(-scale.x, scale.y).Angle();
            float angleBottomRight = new Vertex2f(scale.x, -scale.y).Angle();
            float angleBottomLeft = new Vertex2f(-scale.x, -scale.y).Angle();

            float angleBetween = (_offset - pos).Angle();

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

                    Transform.MoveTransform(Id, new Vertex2f(0, topB - bottomA));
                    break;

                case Side.BOTTOM:
                    topA = global.position.y + global.scale.y;
                    bottomB = collider.position.y - collider.scale.y;

                    Transform.MoveTransform(Id, new Vertex2f(0, bottomB - topA));
                    break;

                case Side.LEFT:
                    rightA = global.position.x + global.scale.x;
                    leftB = collider.position.x - collider.scale.x;

                    Transform.MoveTransform(Id, new Vertex2f(leftB - rightA, 0));
                    break;

                case Side.RIGHT:
                    leftA = global.position.x - global.scale.x;
                    rightB = collider.position.x + collider.scale.x;

                    Transform.MoveTransform(Id, new Vertex2f(rightB - leftA, 0));
                    break; 
            }
        }

        public static void CheckColissions()
        {
            foreach (Collider x in _colliders)
                x.CheckColission();
        }

        private void OnCollision(ColissionEventArgs e)
        {
            EventHandler<ColissionEventArgs> handler = _collisionEvent;
            if (handler == null) { return; }
            handler?.Invoke(null, e);
        }

        public override void DerivedDispose() { }
    }
}