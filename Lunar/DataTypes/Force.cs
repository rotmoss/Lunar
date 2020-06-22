using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public class Force
    {
        public Vector Direction { get => _direction; }
        private Vector _direction;

        private Stopwatch _stopwatch;
        private float _duration;

        public bool IsFinished { get => _stopwatch.ElapsedMilliseconds > _duration && _duration != 0; }

        /// <param name="duration"> The duration in milliseconds.
        public Force(Vector direction, float duration)
        {
            _direction = direction;
            _duration = duration;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void ApplyDrag(float drag)
        {
            _direction *= drag;
        }
    }
}
