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

        public Vertex2d Speed { get => _speed; set => _speed = value; }
        private Vertex2d _speed;
        public Vertex2d Acceleration { get => _acceleration; set => _acceleration = value; }
        private Vertex2d _acceleration;
        public double FrictionConstant { get => _frictionConstant; set => _frictionConstant = value; }
        private double _frictionConstant;

        private static List<Force> _forces = new List<Force>();

        public Force(uint id)
        {
            _id = id;
            _speed = new Vertex2d();
            _acceleration = new Vertex2d();
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
            Vertex2d friction = FastMath.Normalize(-new Vertex2d(_speed.x, _speed.y)) * Time.DeltaTime * _frictionConstant;
            if (!double.IsNaN(friction.x) && !double.IsNaN(friction.y) && friction.Length() < _speed.Length()) { _speed += friction; }
            else { _speed = Vertex2d.Zero; }
        }

        public static void ApplyForces() { foreach (Force x in _forces) x.ApplyForce(); }
        
        public void OnColission(object sender, ColissionEventArgs e)
        {
            Reset(e.side);
        }

        public void Reset()
        {
            _acceleration = Vertex2d.Zero;
            _speed = Vertex2d.Zero;
        }

        public void Reset(Side side)
        {
            _acceleration = side == Side.LEFT || side == Side.RIGHT ? new Vertex2d(0, _acceleration.y) : new Vertex2d(_acceleration.x, 0);
            _speed = side == Side.LEFT || side == Side.RIGHT ? new Vertex2d(0, _speed.y) : new Vertex2d(_speed.x, 0);
        }
    }
}
