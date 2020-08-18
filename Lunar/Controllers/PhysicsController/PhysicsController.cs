using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lunar
{
    public partial class PhysicsController
    {
        private static PhysicsController instance = null;
        public static PhysicsController Instance { get { instance = instance == null ? new PhysicsController() : instance; return instance; } }

        private PhysicsController()
        {
            _speed = new Dictionary<uint, Vector2>();
            _acceleration = new Dictionary<uint, Vector2>();
            _jerk = new Dictionary<uint, Vector2>();
            _friction = new Dictionary<uint, float>();
            _colliders = new Dictionary<uint, List<Transform>>();
            _movable = new Dictionary<uint, bool>();
        }
    }
}
