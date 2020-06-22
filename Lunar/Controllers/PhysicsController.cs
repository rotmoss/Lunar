using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public class PhysicsController
    {
        private static PhysicsController instance = null;
        public static PhysicsController Instance { get { instance = instance == null ? new PhysicsController() : instance; return instance; } }

        Dictionary<uint, List<Force>> _activeForces;
        Dictionary<uint, float> _drag;

        private PhysicsController()
        {
            _activeForces = new Dictionary<uint, List<Force>>();
            _drag = new Dictionary<uint, float>();
        }

        public void AddForce(uint id, Force force)
        {
            if (!_activeForces.ContainsKey(id)) _activeForces.Add(id, new List<Force>());
            _activeForces[id].Add(force);
        }

        public void SetDrag(uint id, float drag = 0)
        {
            if (!_drag.ContainsKey(id)) _drag.Add(id, drag);
            else _drag[id] = drag;
        }

        public void ApplyForces(ref Dictionary<uint, Transform> transforms)
        {

            foreach (uint id in transforms.Keys) 
            {
                if (!_activeForces.ContainsKey(id)) continue;

                for (int i = 0; i < _activeForces[id].Count; i++) {
                    transforms[id] += _activeForces[id][i].Direction * Time.DeltaTime;

                    if (_drag.ContainsKey(id) && _drag[id] != 0) _activeForces[id][i].ApplyDrag(_drag[id]);
                    if (_activeForces[id][i].IsFinished || (_activeForces[id][i].Direction.Length > -0.01f && _activeForces[id][i].Direction.Length < 0.01f)) {
                        _activeForces[id].RemoveAt(i); i--; 
                    }
                }
            }
        }
    }
}
