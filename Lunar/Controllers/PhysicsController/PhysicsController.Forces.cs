using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public partial class PhysicsController
    {
        Dictionary<uint, List<Vector2D>> _activeForces;
        Dictionary<uint, float> _drag;

        public void AddForce(uint id, Vector2D force)
        {
            if (!_activeForces.ContainsKey(id)) _activeForces.Add(id, new List<Vector2D>());
            _activeForces[id].Add(force);
        }

        public void SetDrag(uint id, float drag = 0)
        {
            if (!_drag.ContainsKey(id)) _drag.Add(id, drag);
            else _drag[id] = drag;
        }

        public void ApplyForces(Dictionary<uint, Transform> transforms)
        {

            foreach (uint id in transforms.Keys)
            {
                if (!_activeForces.ContainsKey(id)) continue;

                //Apply drag to all active forces
                if (_drag.ContainsKey(id) && _drag[id] != 0)
                    _activeForces[id] = Vector2D.Multiply(_activeForces[id], _drag[id]);

                //Find vectors that have a length close to 0 and remove them
                for (int i = 0; i < _activeForces[id].Count; i++)
                    if (_activeForces[id][i].Length.AlmostEquals(0)) { _activeForces[id].RemoveAt(i); i--; }

                //Apply the vectors to the transform
                transforms[id] += Vector2D.Sum(Vector2D.Multiply(_activeForces[id], Time.DeltaTime));
            }
        }
    }
}
