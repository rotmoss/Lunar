using System;
using System.Collections.Generic;
using Lunar.Math;
using System.Threading.Tasks;
using Lunar.Scenes;
using Lunar.Stopwatch;
using OpenGL;

namespace Lunar.Physics
{
    public class Force
    {
        private uint _id;

        public Vertex2f Speed { get => _speed; set => _speed = value; }
        private Vertex2f _speed;
        public Vertex2f Acceleration { get => _acceleration; set => _acceleration = value; }
        private Vertex2f _acceleration;
        public float FrictionConstant { get => _frictionConstant; set => _frictionConstant = value; }
        private float _frictionConstant;

        private static List<Force> _forces = new List<Force>();

        public Force(uint id)
        {
            _id = id;
            _speed = new Vertex2f();
            _acceleration = new Vertex2f();
            _frictionConstant = 1;
            _forces.Add(this);
        }

        public void ApplyForce()
        {
            //Calculate speed from acceleration
            _speed += _acceleration * Time.DeltaTime;

            //Calculate position from speed
            Transform.Translate(_id, _speed * Time.DeltaTime);

            //Apply friction force
            Vertex2f friction = FastMath.Normalize(-new Vertex2f(_speed.x, _speed.y)) * Time.DeltaTime * _frictionConstant;
            if (!float.IsNaN(friction.x) && !float.IsNaN(friction.y) && friction.Length() < _speed.Length()) { _speed += friction; }
            else { _speed = Vertex2f.Zero; }
        }

        public static void ApplyForces() { foreach (Force x in _forces) x.ApplyForce(); }
        
        public void OnColission(object sender, ColissionEventArgs e)
        {
            Reset(e.side);
        }

        public void Reset()
        {
            _acceleration = Vertex2f.Zero;
            _speed = Vertex2f.Zero;
        }

        public void Reset(Side side)
        {
            _acceleration = side == Side.LEFT || side == Side.RIGHT ? new Vertex2f(0, _acceleration.y) : new Vertex2f(_acceleration.x, 0);
            _speed = side == Side.LEFT || side == Side.RIGHT ? new Vertex2f(0, _speed.y) : new Vertex2f(_speed.x, 0);
        }
    }
}
