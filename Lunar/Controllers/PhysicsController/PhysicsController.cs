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
            _activeForces = new Dictionary<uint, List<Vector2>>();
            _drag = new Dictionary<uint, float>();
            _colliders = new Dictionary<uint, List<Transform>>();
            _movable = new Dictionary<uint, bool>();
        }
    }
}
