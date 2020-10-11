using System.Diagnostics;

namespace Lunar.Stopwatch
{
    public static class Time
    {
        public static double DeltaTime { get; private set; }
        public static double FrameTime { get; private set; }
        private static System.Diagnostics.Stopwatch _timer = new System.Diagnostics.Stopwatch();

        public static void StartFrameTimer() =>_timer.Start();

        public static  void StopFrameTimer()
        {
            DeltaTime = _timer.Elapsed.TotalSeconds * 10;
            FrameTime = _timer.Elapsed.TotalSeconds;
            _timer.Restart();
        }
    }
}
