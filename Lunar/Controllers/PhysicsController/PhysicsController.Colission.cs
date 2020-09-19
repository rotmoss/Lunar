using System;
using System.Collections.Generic;
using System.Numerics;
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

        public void AddHitBox(uint id, bool isMovable, Vector2 offset, Vector2 size)
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

        //Doesnt work properly. It is supposed to check the global transforms and set the local ones.
        internal void CheckColission(Dictionary<uint, Transform> LocalTransforms, Dictionary<uint, Transform> GlobalTransforms)
        {
            foreach (uint initialId in LocalTransforms.Keys)
            {
                //Check if the enitity has a collider
                if (!_colliders.ContainsKey(initialId)) continue;

                foreach (Transform initial in _colliders[initialId])
                {
                    foreach (uint correspondentId in _colliders.Keys)
                    {
                        //The collider shouldn't collide withn itself
                        if (initialId == correspondentId) continue;
                        if (initialId == SceneController.Instance.GetEntityParent(correspondentId)) continue;
                        if (SceneController.Instance.GetEntityParent(initialId) == correspondentId) continue;

                        foreach (Transform correspondent in _colliders[correspondentId])
                        {
                            Transform a = initial + LocalTransforms[initialId];
                            Transform b = correspondent + LocalTransforms[correspondentId];

                            if (DoesOverlap(a, b)) {
                                //We dont want to move stationary objects
                                if (!_movable.ContainsKey(initialId) || _movable[initialId] == false) { OnCollision(new CollisionEventArgs { id = correspondentId, side = Side.NONE }, initialId); continue; }

                                Side side = CalculateSide(a.position, b.position, b.scale);

                                Move(
                                    LocalTransforms, GlobalTransforms, initialId, correspondentId, 
                                    initial, correspondent, side);

                                _acceleration[initialId] = side == Side.LEFT || side == Side.RIGHT ? new Vector2(0, _acceleration[initialId].Y) : new Vector2(_acceleration[initialId].X, 0);
                                _speed[initialId] = side == Side.LEFT || side == Side.RIGHT ? new Vector2(0, _speed[initialId].Y) : new Vector2(_speed[initialId].X, 0);
                            }
                        }
                    }
                }
            }
        }

        private void Move(Dictionary<uint, Transform> globalTransforms, Dictionary<uint, Transform> localTransforms, uint id, uint colliderId, Transform initial, Transform correspondant, Side side)
        {
            float topA, bottomA, leftA, rightA, topB, bottomB, leftB, rightB;

            switch (side)
            {
                case Side.TOP:
                    bottomA = globalTransforms[id].position.Y + initial.position.Y - initial.scale.Y;
                    topB = globalTransforms[colliderId].position.Y + correspondant.position.Y + correspondant.scale.Y;

                    localTransforms[id] += new Vector2(0, topB - bottomA);
                    OnCollision(new CollisionEventArgs { id = colliderId, side = Side.TOP }, id);
                    break;

                case Side.BOTTOM:
                    topA = globalTransforms[id].position.Y + initial.position.Y + initial.scale.Y;
                    bottomB = globalTransforms[colliderId].position.Y + correspondant.position.Y - correspondant.scale.Y;

                    localTransforms[id] += new Vector2(0, bottomB - topA);
                    OnCollision(new CollisionEventArgs { id = colliderId, side = Side.BOTTOM }, id);
                    break;

                case Side.LEFT:
                    rightA = globalTransforms[id].position.X + initial.position.X + initial.scale.X;
                    leftB = globalTransforms[colliderId].position.X + correspondant.position.X - correspondant.scale.X;

                    localTransforms[id] += new Vector2(leftB - rightA, 0);
                    OnCollision(new CollisionEventArgs { id = colliderId, side = Side.LEFT }, id);
                    break;

                case Side.RIGHT:
                    leftA = globalTransforms[id].position.X + initial.position.X - initial.scale.X;
                    rightB = globalTransforms[colliderId].position.X + correspondant.position.X + correspondant.scale.X;

                    localTransforms[id] += new Vector2(rightB - leftA, 0);
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
                if (((Script)d.Target).Id != id)
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