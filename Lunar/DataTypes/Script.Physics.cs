using System;
using System.Numerics;

namespace Lunar
{
    public partial class Script
    {
        public Vector2 Acceleration { get => PhysicsController.Instance.GetAcceleration(Id); set => PhysicsController.Instance.SetAcceleration(Id, value); }
        public Vector2 Speed { get => PhysicsController.Instance.GetSpeed(Id); set => PhysicsController.Instance.SetSpeed(Id, value); }
        public float Friction { get => PhysicsController.Instance.GetFriction(Id); set => PhysicsController.Instance.SetFriction(Id, value); }
        public void AddHitbox(bool isMovable, Transform hitbox) => PhysicsController.Instance.AddHitBox(Id, isMovable, hitbox);
        public EventHandler<PhysicsController.CollisionEventArgs> CollisionEvent { get => PhysicsController.Instance.CollisionEvent; set => PhysicsController.Instance.CollisionEvent = value; }
    }
}
