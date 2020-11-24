using System;
using System.Threading.Tasks;
using SDL2;
using Lunar.Scenes;
using Lunar.Physics;
using Lunar.Input;
using Lunar.Scripts;
using Lunar.Audio;
using Lunar.Compiler;

namespace Lunar
{ 
    public class Lunar
    {
        private static GameWindow _window;

        static void Main(string[] args)
        {
            Init();

            while (true)
                Update();
        }

        public static void Init()
        {
            Task<System.Reflection.Assembly> assembly = AssemblyCompiler.CompileScripts();

            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }
            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            Console.WriteLine("SDL Initiated!");

            _window = new GameWindow(1280, 720, false);
            Window.CreateFramebuffer();
            Window.AddLayers(new string[] { "background", "sprite"});

            Console.WriteLine("Window Created!");

            InputController.OnWindowSizeChanged += _window.UpdateWindowSize;
            InputController.OnKeyDown += _window.OnKeyDown;

            Mixer.Init();

            InputController.Init();
            InputController.OnWindowClose += OnWindowClose;

            assembly.Wait();
            Script.Assembly = assembly.Result;
            Scene.LoadScene("start.xml");

            Script.InitScripts();
            Script.LateInitScripts();
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

        static void OnWindowClose(object sender, EventArgs eventArgs)
        {
            Mixer.Dispose();
            Window.Close();
            Environment.Exit(0);
        }
    }
}
