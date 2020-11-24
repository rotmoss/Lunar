using System.Diagnostics;

namespace Lunar
{
    public static class Time
    {
        public static double DeltaTime { get; private set; }
        public static double FrameTime { get; private set; }
        private static Stopwatch _timer = new Stopwatch();

        public static void StartFrameTimer()
        {
            if (_timer.IsRunning) _timer.Restart();
            else _timer.Start();
        }

        public static  void StopFrameTimer()
        {
            DeltaTime = _timer.Elapsed.TotalSeconds * 10;
            FrameTime = _timer.Elapsed.TotalSeconds;
            _timer.Stop();
            _timer.Reset();
        }
    }
}
