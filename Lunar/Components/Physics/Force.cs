using System;
using System.Collections.Generic;
using Lunar.Math;
using System.Threading.Tasks;
using OpenGL;

namespace Lunar.Physics
{
    public class Force : Component<Force>
    {
        public Vertex2f Speed { get => _speed; set => _speed = value; }
        private Vertex2f _speed;

        public Vertex2f Acceleration { get => _acceleration; set => _acceleration = value; }
        private Vertex2f _acceleration;

        public float FrictionConstant { get => _frictionConstant; set => _frictionConstant = value; }
        private float _frictionConstant;

        public Force(float frictionConstant = 1) : base()
        {
            _speed = new Vertex2f();
            _acceleration = new Vertex2f();
            _frictionConstant = frictionConstant;
        }

        public void ApplyForce()
        {
            //Calculate speed from acceleration
            _speed += _acceleration * Time.DeltaTime;

            //Calculate position from speed
            Transform.MoveTransform(_id, _speed * Time.DeltaTime);

            //Apply friction force
            Vertex2f friction = VertexExtentions.Normalize(-new Vertex2f(_speed.x, _speed.y)) * Time.DeltaTime * _frictionConstant;
            if (!float.IsNaN(friction.x) && !float.IsNaN(friction.y) && friction.Length() < _speed.Length()) { _speed += friction; }
            else { _speed = Vertex2f.Zero; }
        }

        public static void ApplyForces() 
        { 
            foreach (Force x in _components) 
                x.ApplyForce(); 
        }

        public void OnColission(object sender, ColissionEventArgs e)
        {
            _acceleration = e.side == Side.LEFT || e.side == Side.RIGHT ? new Vertex2f(0, _acceleration.y) : new Vertex2f(_acceleration.x, 0);
            _speed = e.side == Side.LEFT || e.side == Side.RIGHT ? new Vertex2f(0, _speed.y) : new Vertex2f(_speed.x, 0);
        }

        public override void DisposeChild() { }
    }
}
