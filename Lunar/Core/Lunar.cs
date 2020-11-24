using System;
using System.Threading.Tasks;
using SDL2;
using Lunar.Physics;
using Lunar.Graphics;
using Lunar.Input;
using Lunar.Scripts;
using Lunar.Audio;
using Lunar.Compiler;
using System.Reflection;

namespace Lunar
{ 
    public class Lunar
    {
        private static GameWindow _window;
        private static Scene _scene;

        static void Main(string[] args)
        {
            Init();

            while (true)
                Update();
        }

        public static void Init()
        {
            Task<Assembly> assembly = AssemblyCompiler.CompileScripts();

            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }
            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            Console.WriteLine("SDL Initiated!");

            _window = new GameWindow(1280, 720, false);
            Renderer.AddLayers(new string[] { "background", "sprite"});

            Console.WriteLine("Window Created!");

            InputController.Init();
            Mixer.Init();

            InitEvents();

            assembly.Wait();
            Script.Assembly = assembly.Result;
            _scene = new Scene("start.xml");

            Script.InitScripts();
            Script.LateInitScripts();
        }

        public static void InitEvents()
        {
            InputController.OnWindowClose += OnWindowClose;
            InputController.OnWindowSizeChanged += _window.UpdateSize;
            InputController.OnKeyDown += _window.OnKeyDown;

            Gameobject.OnDispose += Sample.OnGameobjectDispose;
            Gameobject.OnDispose += Force.OnGameobjectDispose;
            Gameobject.OnDispose += Collider.OnGameobjectDispose;
            Gameobject.OnDispose += Script.OnGameobjectDispose;
            Gameobject.OnDispose += Sprite.OnGameobjectDispose;
            Gameobject.OnDispose += Text.OnGameobjectDispose;
            Gameobject.OnDispose += Animation.OnGameobjectDispose;
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
            Renderer.Render();

            //Update all scripts again
            Script.PostRenderUpdateScripts();

            //Present FrameBuffer
            _window.SwapBuffer();

            Time.StopFrameTimer();
        }

        static void OnWindowClose(object sender, EventArgs eventArgs)
        {
            _scene.Dispose();
            _window.Dispose();
            Mixer.Dispose();
            Environment.Exit(0);
        }
    }
}
