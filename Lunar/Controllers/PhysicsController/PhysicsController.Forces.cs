using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OpenGL;

namespace Lunar
{
    public partial class PhysicsController
    {
        const float DegreesToRadians = MathF.PI / 180f;

        Dictionary<uint, List<Vector2>> _activeForces;
        Dictionary<uint, float> _drag;

        public void AddForce(uint id, Vector2 force)
        {
            if (!_activeForces.ContainsKey(id)) _activeForces.Add(id, new List<Vector2>());
            _activeForces[id].Add(force);
        }

        public void AddForce(uint id, float length, float angle)
        {
            if (!_activeForces.ContainsKey(id)) _activeForces.Add(id, new List<Vector2>());
            _activeForces[id].Add(new Vector2(FastMath.Cos(angle * DegreesToRadians) * length, FastMath.Sin(angle * DegreesToRadians) * length));
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
                    _activeForces[id] = FastMath.MultiplyVectors(_activeForces[id], _drag[id]);

                //Find vectors that have a length close to 0 and remove them
                for (int i = 0; i < _activeForces[id].Count; i++)
                    if (_activeForces[id][i].Length().AlmostEquals(0)) { _activeForces[id].RemoveAt(i); i--; }

                //calculate new transform from forces and add it
                transforms[id] += FastMath.SumVectors(_activeForces[id]) * Time.DeltaTime;  
            }
        }

        public void DrawColliders(Dictionary<uint, Transform> transforms)
        {
            Gl.LineWidth(2);

            foreach (uint id in transforms.Keys)
            {
                if (!_colliders.ContainsKey(id)) continue;

                Gl.Begin(PrimitiveType.LineLoop);
                Gl.Vertex2((transforms[id].position.X + _colliders[id].position.X - _colliders[id].scale.X) / 480, (transforms[id].position.Y + _colliders[id].position.Y - _colliders[id].scale.Y) / 270);
                Gl.Vertex2((transforms[id].position.X + _colliders[id].position.X - _colliders[id].scale.X) / 480, (transforms[id].position.Y + _colliders[id].position.Y + _colliders[id].scale.Y) / 270);
                Gl.Vertex2((transforms[id].position.X + _colliders[id].position.X + _colliders[id].scale.X) / 480, (transforms[id].position.Y + _colliders[id].position.Y + _colliders[id].scale.Y) / 270);
                Gl.Vertex2((transforms[id].position.X + _colliders[id].position.X + _colliders[id].scale.X) / 480, (transforms[id].position.Y + _colliders[id].position.Y - _colliders[id].scale.Y) / 270);
                Gl.End();
            }
        }
    }
}
