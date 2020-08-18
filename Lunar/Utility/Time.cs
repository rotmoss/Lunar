using System.Diagnostics;

namespace Lunar
{
    static class Time
    {
        public static float DeltaTime { get; private set; }
        public static float FrameRate { get; private set; }
        private static Stopwatch _timer = new Stopwatch();

        public static void StartFrameTimer() =>_timer.Start();

        public static  void StopFrameTimer()
        {
            DeltaTime = (float)_timer.Elapsed.TotalSeconds * 10;
            FrameRate = 1 / (DeltaTime / 10);
            _timer.Restart();
        }
    }
}
