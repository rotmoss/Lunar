using System;
using Lunar.Scenes;
using Lunar.Input;
using System.Windows;
using System.Windows.Controls;
using Lunar.Scripts;
using Lunar.Audio;
using SDL2;
using System.Threading.Tasks;
using Lunar.Physics;
using Lunar.Compiler;

namespace Lunar.Editor
{
    public static class LunarEngine
    {
        public static LunarWindow _window;

        public static void Init(IntPtr handle, Canvas canvas)
        {
            Task task = Task.Run(() => LoadScene());

            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }
            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            Rect rect = canvas.GetAbsolutePlacement(false);
            _window = new LunarWindow(handle, (int)canvas.ActualWidth, (int)canvas.ActualHeight, (int)rect.X, (int)rect.Y);

            Window.CreateFramebuffer();
        
            Mixer.Init();
            InputController.Init();

            task.Wait();

            Script.InitScripts();
            Script.LateInitScripts();
        }

        public static async void LoadScene()
        {
            Script.Assembly = await AssemblyCompiler.CompileScripts();
            Scene.LoadScene("start.xml");
        }

        public static void Update()
        {
            Time.StartFrameTimer();

            //Invoke Input-Events
            InputController.PollInputs();

            //Update all scripts
            Script.UpdateScripts();

            //Update Transforms
            Force.ApplyForces();

            //Check Colissions
            Collider.CheckColissions();

            //Update all scripts again
            Script.LateUpdateScripts();

            //Use Transfroms to Translate Graphics Data
            GraphicsComponent.TranslateBuffers();

            Animation.Animate(Time.FrameTime);

            //Render Graphics
            Window.Render();

            //Update all scripts again
            Script.PostRenderUpdateScripts();

            //Present FrameBuffer
            Window.SwapBuffer();

            Time.StopFrameTimer();
        }

        public static void Render()
        {
            //Invoke Input-Events
            InputController.PollInputs();

            //Render Graphics
            //Window.Render();

            //Present FrameBuffer
            //Window.SwapBuffer();
        }

        public static void UpdateWindow(int x, int y, int w, int h)
        {        
            _window.Resize(x, y, w, h);
            Lunar.Update();
        }

        public static void Close()
        {
            Mixer.Dispose();
            Window.Close();
            Environment.Exit(0);
        }
    }
}
