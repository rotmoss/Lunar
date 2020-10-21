using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lunar.Scenes;
using Lunar.Stopwatch;

namespace Lunar.Physics
{
    public class Force
    {
        private uint _id;

        public Vector2 Speed { get => _speed; set => _speed = value; }
        private Vector2 _speed;
        public Vector2 Acceleration{ get => _acceleration; set => _acceleration = value; }
        private Vector2 _acceleration;
        public float FrictionConstant { get => _frictionConstant; set => _frictionConstant = value; }
        private float _frictionConstant;

        private static List<Force> _forces = new List<Force>();

        public Force(uint id)
        {
            _id = id;
            _speed = new Vector2();
            _acceleration = new Vector2();
            _frictionConstant = 1;
            _forces.Add(this);
        }

        public void ApplyForce()
        {
            //Calculate speed from acceleration
            _speed += _acceleration * (float)Time.DeltaTime;

            //Calculate position from speed
            Transform.Translate(_id, _speed * (float)Time.DeltaTime);

            //Apply friction force
            Vector2 friction = Vector2.Normalize(-new Vector2(_speed.X, _speed.Y)) * (float)Time.DeltaTime * _frictionConstant;
            if (!float.IsNaN(friction.X) && !float.IsNaN(friction.Y) && friction.Length() < _speed.Length()) { _speed += friction; }
            else { _speed = Vector2.Zero; }
        }

        public static void ApplyForces() => Parallel.ForEach(_forces, x => x.ApplyForce());
        

        public void OnColission(object sender, ColissionEventArgs e)
        {
            Reset(e.side);
        }

        public void Reset()
        {
            _acceleration = Vector2.Zero;
            _speed = Vector2.Zero;
        }

        public void Reset(Side side)
        {
            _acceleration = side == Side.LEFT || side == Side.RIGHT ? new Vector2(0, _acceleration.Y) : new Vector2(_acceleration.X, 0);
            _speed = side == Side.LEFT || side == Side.RIGHT ? new Vector2(0, _speed.Y) : new Vector2(_speed.X, 0);
        }
    }
}
