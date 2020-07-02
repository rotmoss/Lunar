using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public partial class PhysicsController
    {
        public class CollisionEventArgs : EventArgs
        {
            public  string name;
            public uint id;
        }

        Dictionary<uint, Point[]> _colliders;
        Dictionary<uint, Transform> _colliderOffset;
        EventHandler<CollisionEventArgs> _collisionEvent;

        public void AddHitBox(uint id, Point[] polygon)
        {
            if (!_colliders.ContainsKey(id)) _colliders.Add(id, new Point[0]);
            _colliders[id] = polygon;
        }

        public Point[] GenSquareHitbox(float w, float h)
        {
            return new Point[] { new Point(-w, -h), new Point(-w, h), new Point(w, h), new Point(w, -h) };
        }

        public bool isColliding(Transform transform, uint id)
        {
            bool result = false;
            Transform offset;

            if (!_colliderOffset.ContainsKey(id)) offset = transform;
            else offset = transform + _colliderOffset[id];

            foreach (uint collider in _colliders.Keys)
            {
                if (id == collider) continue;

                foreach (Point p in _colliders[id])
                {
                    if (!(p + offset).isInside(_colliders[collider])) continue;
                    OnCollision(new CollisionEventArgs { id = collider }, id);
                    result = true;
                }
            }
            return result;
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
    }
}
