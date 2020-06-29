using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public partial class PhysicsController
    {
        private static PhysicsController instance = null;
        public static PhysicsController Instance { get { instance = instance == null ? new PhysicsController() : instance; return instance; } }

        private PhysicsController()
        {
            _activeForces = new Dictionary<uint, List<Vector2D>>();
            _drag = new Dictionary<uint, float>();
        }
    }
}
