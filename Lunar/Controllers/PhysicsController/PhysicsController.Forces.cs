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

        Dictionary<uint, Vector2> _speed;
        Dictionary<uint, Vector2> _acceleration;
        Dictionary<uint, Vector2> _jerk;
        Dictionary<uint, float> _friction;

        public void SetSpeed(uint id, Vector2 v)
        {
            if (!_speed.ContainsKey(id)) _speed.Add(id, Vector2.Zero);
            _speed[id] = v;
        }

        public void SetSpeed(uint id, float length, float angle)
        {
            if (!_speed.ContainsKey(id)) _speed.Add(id, Vector2.Zero);
            _speed[id] = new Vector2(FastMath.Cos(angle * DegreesToRadians) * length, FastMath.Sin(angle * DegreesToRadians) * length);
        }

        public void SetAcceleration(uint id, Vector2 v)
        {
            if (!_acceleration.ContainsKey(id)) _acceleration.Add(id, Vector2.Zero);
            _acceleration[id] = v;
        }

        public void SetAcceleration(uint id, float length, float angle)
        {
            if (!_acceleration.ContainsKey(id)) _acceleration.Add(id, Vector2.Zero);
            _acceleration[id] = new Vector2(FastMath.Cos(angle * DegreesToRadians) * length, FastMath.Sin(angle * DegreesToRadians) * length);
        }

        public void Setjerk(uint id, Vector2 v)
        {
            if (!_jerk.ContainsKey(id)) _jerk.Add(id, Vector2.Zero);
            _jerk[id] = v;
        }

        public void SetFriction(uint id, float drag = 0)
        {
            if (!_friction.ContainsKey(id)) _friction.Add(id, drag);
            else _friction[id] = drag;
        }

        internal void ApplyForces(Dictionary<uint, Transform> transforms)
        {
            foreach (uint id in transforms.Keys)
            {
                if (!_speed.ContainsKey(id)) _speed.Add(id, Vector2.Zero);
                if (!_acceleration.ContainsKey(id)) _acceleration.Add(id, Vector2.Zero);
                if (!_jerk.ContainsKey(id)) _jerk.Add(id, Vector2.Zero);
                if (!_friction.ContainsKey(id)) _friction.Add(id, 0);

                _acceleration[id] += _jerk[id] * Time.DeltaTime;

                //Calculate speed from acceleration
                _speed[id] += _acceleration[id] * Time.DeltaTime;

                //Calculate position from speed
                transforms[id] += _speed[id] * Time.DeltaTime;

                Vector2 friction = Vector2.Normalize(new Vector2(-_speed[id].X, -_speed[id].Y)) * Time.DeltaTime * _friction[id];
                if (!float.IsNaN(friction.X) && !float.IsNaN(friction.Y) && friction.Length() < _speed[id].Length()) { _speed[id] += friction; }
                else { _speed[id] = Vector2.Zero; }
            }
        }

        internal void DrawColliders(Dictionary<uint, Transform> transforms)
        {
            Gl.LineWidth(2);

            foreach (uint id in transforms.Keys)
            {
                if (!_colliders.ContainsKey(id)) continue;
                foreach (Transform collider in _colliders[id])
                {
                    Gl.Begin(PrimitiveType.LineLoop);
                    Gl.Vertex2((transforms[id].position.X + collider.position.X - collider.scale.X) / 480f, (transforms[id].position.Y + collider.position.Y - collider.scale.Y) / 270f);
                    Gl.Vertex2((transforms[id].position.X + collider.position.X - collider.scale.X) / 480f, (transforms[id].position.Y + collider.position.Y + collider.scale.Y) / 270f);
                    Gl.Vertex2((transforms[id].position.X + collider.position.X + collider.scale.X) / 480f, (transforms[id].position.Y + collider.position.Y + collider.scale.Y) / 270f);
                    Gl.Vertex2((transforms[id].position.X + collider.position.X + collider.scale.X) / 480f, (transforms[id].position.Y + collider.position.Y - collider.scale.Y) / 270f);
                    Gl.End();
                }
            }
        }
    }
}
