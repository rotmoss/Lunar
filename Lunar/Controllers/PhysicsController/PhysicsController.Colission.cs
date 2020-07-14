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
        }

        private Dictionary<uint, bool> _movable;
        private Dictionary<uint, Transform> _colliders;
        public EventHandler<CollisionEventArgs> CollisionEvent { get => _collisionEvent; set => _collisionEvent = value; }
        private EventHandler<CollisionEventArgs> _collisionEvent;

        public void AddHitBox(uint id, Vector2 size, Vector2 offset)
        {
            if (!_colliders.ContainsKey(id)) _colliders.Add(id, new Transform(0,0));
            _colliders[id] = new Transform(offset, size);
        }
        public void AddHitBox(uint id, Transform collider)
        {
            if (!_colliders.ContainsKey(id)) _colliders.Add(id, new Transform(0, 0));
            _colliders[id] = collider;
        }
        public void IsMovable(uint id, bool movable)
        {
            if (!_movable.ContainsKey(id)) _movable.Add(id, movable);
            _movable[id] = true;
        }

        public void CheckColission(Dictionary<uint, Transform> transforms)
        {
            foreach (uint id in transforms.Keys)
            {
                //We dont want to move stationary objects
                if (!_movable.ContainsKey(id) || _movable[id] == false) continue;

                switch (isColliding(id, transforms, out uint colliderId))
                {
                    case Side.TOP:
                        float bottomA = transforms[id].position.Y + _colliders[id].position.Y - _colliders[id].scale.Y;
                        float topB = transforms[colliderId].position.Y + _colliders[colliderId].position.Y + _colliders[colliderId].scale.Y;

                        transforms[id] += new Vector2(0, topB - bottomA);
                        break;
                    case Side.BOTTOM:
                        float topA = transforms[id].position.Y + _colliders[id].position.Y + _colliders[id].scale.Y;
                        float bottomB = transforms[colliderId].position.Y + _colliders[colliderId].position.Y - _colliders[colliderId].scale.Y;

                        transforms[id] += new Vector2(0, bottomB - topA);
                        break;
                    case Side.LEFT:
                        float rightA = transforms[id].position.X + _colliders[id].position.X + _colliders[id].scale.X;
                        float leftB = transforms[colliderId].position.X + _colliders[colliderId].position.X - _colliders[colliderId].scale.X;

                        transforms[id] += new Vector2(leftB - rightA, 0);
                        break;
                    case Side.RIGHT:
                        float leftA = transforms[id].position.X + _colliders[id].position.X - _colliders[id].scale.X;
                        float rightB = transforms[colliderId].position.X + _colliders[colliderId].position.X + _colliders[colliderId].scale.X;

                        transforms[id] += new Vector2(rightB - leftA, 0);
                        break;
                }
            }
        }

        public Side isColliding(uint id, Dictionary<uint, Transform> transforms, out uint collider)
        {
            collider = 0;
            //Check if the enitity has a collider
            if (!_colliders.ContainsKey(id))  return Side.NONE;

            foreach (uint colliderID in _colliders.Keys)
            {
                //The collider shouldn't collide withn itself
                if (id == colliderID) continue;

                Transform a = _colliders[id] + transforms[id];
                Transform b = _colliders[colliderID] + transforms[colliderID];

                if (DoesOverlap(a, b))
                {
                    collider = colliderID;
                    OnCollision(new CollisionEventArgs { id = colliderID }, id);
                    return CalculateSide(a.position, b.position);
                }
            }
            return Side.NONE;
        }

        public Side CalculateSide(Vector2 a, Vector2 b)
        {
            float angleBetween = (a - b).Angle();

            // the angle between the two vectors is between -45 and 45 degrees
            if (angleBetween > 0.7853981f && angleBetween < 2.3561944f)
                return Side.RIGHT;

            // the angle between the two vectors between 135 and 225 degrees
            if (angleBetween < -0.7853981f && angleBetween > -2.3561944f)
                return Side.LEFT;

            // the angle between the two vectors is between 45 and 135 degrees
            if (angleBetween > -0.7853981f && angleBetween < 0.7853981f)
                return Side.TOP;

            if (angleBetween < -2.3561944f || angleBetween > 2.3561944f)
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
            //The sides of the rectangles
            float leftA, leftB;
            float rightA, rightB;
            float topA, topB;
            float bottomA, bottomB;

            //Calculate the sides of rect A
            leftA = a.position.X - a.scale.X;
            rightA = a.position.X + a.scale.X;
            bottomA = a.position.Y - a.scale.Y;
            topA = a.position.Y + a.scale.Y;

            //Calculate the sides of rect B
            leftB = b.position.X - b.scale.X; ;
            rightB = b.position.X + b.scale.X;
            bottomB = b.position.Y - b.scale.Y;
            topB = b.position.Y + b.scale.Y;

            //If any of the sides from A are outside of B
            if (bottomA >= topB)
                return false;

            if (topA <= bottomB)
                return false;

            if (rightA <= leftB)
                return false;

            if (leftA >= rightB)
                return false;

            //If none of the sides from A are outside B
            return true;
        }
    }
}