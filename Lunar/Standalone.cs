using System;
using System.Threading.Tasks;
using Lunar.Input;
using Lunar.ECS;
using Lunar.SDL;
using Lunar.GL;
using Lunar.Compiler;
using System.Reflection;
using Lunar.ECS.Components;

namespace Lunar
{
    public static class Standalone
    {
        public static void Main(string[] args)
        {
            LunarEngine Lunar = new LunarEngine(new RenderToFrameBuffer(), new StandaloneWindowContext(new ViewportSize { W = 1280, H = 720 }));

            while (true)
            {
                Time.StartFrameTimer();

                InputController.PollInputs();

                Lunar.Update();

                Lunar.Render();

                Time.StopFrameTimer();
            }
        }
    }
}
