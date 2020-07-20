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
            foreach (uint initialId in transforms.Keys)
            {
                //We dont want to move stationary objects
                if (!_movable.ContainsKey(initialId) || _movable[initialId] == false) continue;

                //Check if the enitity has a collider
                if (!_colliders.ContainsKey(initialId)) continue;

                foreach (Transform initial in _colliders[initialId])
                {
                    foreach (uint correspondantId in _colliders.Keys)
                    {
                        //The collider shouldn't collide withn itself
                        if (initialId == correspondantId) continue;

                        foreach (Transform correspondant in _colliders[correspondantId])
                        {
                            Transform a = initial + transforms[initialId];
                            Transform b = correspondant + transforms[correspondantId];

                            if (DoesOverlap(a, b))
                                Move(
                                    initialId, correspondantId, transforms, 
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

        private Side CalculateSide(Vector2 aPos, Vector2 bPos, Vector2 bScale)
        {
            //Calulate the angle from the center of b to each of its corners
            float angleTopRight =    new Vector2(bScale.X,  bScale.Y).Angle();
            float angleTopLeft =     new Vector2(-bScale.X, bScale.Y).Angle();
            float angleBottomRight = new Vector2(bScale.X,  -bScale.Y).Angle();
            float angleBottomLeft =  new Vector2(-bScale.X, -bScale.Y).Angle();

            float angleBetween = (aPos - bPos).Angle();

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