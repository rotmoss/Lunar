using System.Diagnostics;

namespace Lunar
{
    public static class Time
    {
        public static double DeltaTime { get; private set; }
        private static Stopwatch _timer = new Stopwatch();

        public static void StartFrameTimer() =>_timer.Start();

        public static  void StopFrameTimer()
        {
            DeltaTime = _timer.Elapsed.TotalSeconds * 10;
            _timer.Restart();
        }
    }
}
