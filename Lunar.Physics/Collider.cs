using System;
using System.Collections.Generic;
using System.Numerics;
using Lunar.Math;
using Lunar.Scene;
using Lunar.Graphics;

namespace Lunar.Physics
{
    public enum Side
    {
        NONE, TOP, BOTTOM, LEFT, RIGHT
    }

    public class ColissionEventArgs : EventArgs
    {
        public bool movable;
        public Collider collider;
        public Collider reciever;
        public Side side;
    }

    public class Collider
    {
        private uint id;

        private bool movable;
        private Vector2 offset;
        private Vector2 size;

        public EventHandler<ColissionEventArgs> CollisionEvent { get => _collisionEvent; set => _collisionEvent = value; }
        private EventHandler<ColissionEventArgs> _collisionEvent;

        private static List<Collider> _colliders = new List<Collider>();

        public Collider(uint Id, Vector2 offset, Vector2 size, bool movable = false)
        {
            id = Id;
            this.movable = movable;
            this.offset = offset;
            this.size = size;

            _colliders.Add(this);
        }

        public static void CheckColissions()
        {
            foreach (Collider collider in _colliders)
            {
                if(collider.movable) collider.CheckColission();
            }
        }

        public void CheckColission()
        {
            foreach (Collider collider in _colliders)
            {
                uint id = this.id;
                do
                {
                    if (id == collider.id) break;
                    id = SceneController.Instance.GetEntityParent(id);
                } while (id != 0);
                if (id == collider.id) continue;

                Transform a = new Transform(offset, size) + Transform.GetGlobalTransform(this.id);
                Transform b = new Transform(collider.offset, collider.size) + Transform.GetGlobalTransform(collider.id);

                if (DoesOverlap(a, b))
                {
                    Side side = CalculateSide(b.position, b.scale);
                    MoveTransform(side, a, b);
                    OnCollision(new ColissionEventArgs { movable = movable, collider = collider, reciever = this, side = side });
                }
            }
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

        public static void DrawColliders()
        {
            foreach (Collider collider in _colliders)
            {
                Transform transform = new Transform(collider.offset, collider.size) + Transform.GetGlobalTransform(collider.id);
                Window.DrawQuad(false, transform.position.X - transform.scale.X, transform.position.Y - transform.scale.Y, transform.position.X + transform.scale.X, transform.position.Y + transform.scale.Y, 0, 0, 0);
            }
        }
    }
}