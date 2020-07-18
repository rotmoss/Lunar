using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public partial class PhysicsController
    {
        public enum Side
        {
            NONE, TOP, BOTTOM, LEFT, RIGHT
        }

        public class CollisionEventArgs : EventArgs
        {
            public uint id;
            public Side side;
        }

        private Dictionary<uint, bool> _movable;
        private Dictionary<uint, List<Transform>> _colliders;
        public EventHandler<CollisionEventArgs> CollisionEvent { get => _collisionEvent; set => _collisionEvent = value; }
        private EventHandler<CollisionEventArgs> _collisionEvent;

        public void AddHitBox(uint id, bool isMovable, Vector2 size, Vector2 offset)
        {
            if (!_colliders.ContainsKey(id)) _colliders.Add(id, new List<Transform>());
            _colliders[id].Add(new Transform(offset, size));

            if (!_movable.ContainsKey(id)) _movable.Add(id, isMovable);
            _movable[id] = isMovable;
        }
        public void AddHitBox(uint id, bool isMovable, Transform collider)
        {
            if (!_colliders.ContainsKey(id)) _colliders.Add(id, new List<Transform>());
            _colliders[id].Add(collider);

            if (!_movable.ContainsKey(id)) _movable.Add(id, isMovable);
            _movable[id] = isMovable;
        }

        internal void CheckColission(Dictionary<uint, Transform> transforms)
        {
            foreach (uint id in transforms.Keys)
            {
                //We dont want to move stationary objects
                if (!_movable.ContainsKey(id) || _movable[id] == false) continue;

                //Check if the enitity has a collider
                if (!_colliders.ContainsKey(id)) continue;

                foreach (Transform initial in _colliders[id])
                {
                    foreach (uint collider in _colliders.Keys)
                    {
                        //The collider shouldn't collide withn itself
                        if (id == collider) continue;

                        foreach (Transform correspondant in _colliders[collider])
                        {
                            Transform a = initial + transforms[id];
                            Transform b = correspondant + transforms[collider];

                            if (DoesOverlap(a, b))
                                Move(
                                    id, collider, transforms, 
                                    initial, correspondant, 
                                    CalculateSide(a.position, b.position, b.scale));
                            
                        }
                    }
                }
            }
        }

        private void Move(uint id, uint colliderId, Dictionary<uint, Transform> transforms, Transform initial, Transform correspondant, Side side)
        {
            float topA, bottomA, leftA, rightA, topB, bottomB, leftB, rightB;

            switch (side)
            {
                case Side.TOP:
                    bottomA = transforms[id].position.Y + initial.position.Y - initial.scale.Y;
                    topB = transforms[colliderId].position.Y + correspondant.position.Y + correspondant.scale.Y;

                    transforms[id] += new Vector2(0, topB - bottomA);
                    OnCollision(new CollisionEventArgs { id = colliderId, side = Side.TOP }, id);
                    break;

                case Side.BOTTOM:
                    topA = transforms[id].position.Y + initial.position.Y + initial.scale.Y;
                    bottomB = transforms[colliderId].position.Y + correspondant.position.Y - correspondant.scale.Y;

                    transforms[id] += new Vector2(0, bottomB - topA);
                    OnCollision(new CollisionEventArgs { id = colliderId, side = Side.BOTTOM }, id);
                    break;

                case Side.LEFT:
                    rightA = transforms[id].position.X + initial.position.X + initial.scale.X;
                    leftB = transforms[colliderId].position.X + correspondant.position.X - correspondant.scale.X;

                    transforms[id] += new Vector2(leftB - rightA, 0);
                    OnCollision(new CollisionEventArgs { id = colliderId, side = Side.LEFT }, id);
                    break;

                case Side.RIGHT:
                    leftA = transforms[id].position.X + initial.position.X - initial.scale.X;
                    rightB = transforms[colliderId].position.X + correspondant.position.X + correspondant.scale.X;

                    transforms[id] += new Vector2(rightB - leftA, 0);
                    OnCollision(new CollisionEventArgs { id = colliderId, side = Side.RIGHT }, id);
                    break;
            }
        }

        private void isColliding(uint id, Dictionary<uint, Transform> transforms)
        {

        }

        private Side CalculateSide(Vector2 a, Vector2 b, Vector2 bScale)
        {
            float angleTopRight =    new Vector2(bScale.X,  bScale.Y).Angle();
            float angleTopLeft =     new Vector2(-bScale.X, bScale.Y).Angle();
            float angleBottomRight = new Vector2(bScale.X,  -bScale.Y).Angle();
            float angleBottomLeft =  new Vector2(-bScale.X, -bScale.Y).Angle();

            float angleBetween = (a - b).Angle();

            // the angle between the two vectors is between -45 and 45 degrees
            if (angleBetween < angleBottomRight && angleBetween > angleTopRight)
                return Side.RIGHT;

            // the angle between the two vectors between 135 and 225 degrees
            if (angleBetween < angleTopLeft && angleBetween > angleBottomLeft)
                return Side.LEFT;

            // the angle between the two vectors is between 45 and 135 degrees
            if (angleBetween < angleTopRight && angleBetween > angleTopLeft)
                return Side.TOP;

            if (angleBetween < angleBottomLeft || angleBetween > angleBottomRight)
                return Side.BOTTOM;

            return Side.NONE;
        }

        private void OnCollision(CollisionEventArgs e, uint id)
        {
            EventHandler<CollisionEventArgs> handler = _collisionEvent;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (((Script)d.Target)._id != id && instance != null)
                { handler -= (EventHandler<CollisionEventArgs>)d; }
            }

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
    }
}