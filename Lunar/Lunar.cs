using OpenGL;
using SDL2;
using System;
using Lunar.Scene;
using Lunar.Physics;
using Lunar.Graphics;
using Lunar.Input;
using Lunar.Scripts;
using Lunar.Stopwatch;

namespace Lunar
{ 
    public class Lunar
    {
        public static bool DrawColliders { get => _drawColliders; set => _drawColliders = value; }
        private static bool _drawColliders;

        static void Main(string[] args)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            _drawColliders = false;

            var assemblyAwaiter = AssemblyCompiler.Instance.CompileScripts();
            InputController.Init();

            InputController.OnWindowClose += OnWindowClose;
            InputController.OnWindowSizeChanged += OnWindowSizeChanged;
            InputController.OnKeyDown += OnKeyDown;

            Window.Init(1280, 720, false);

            assemblyAwaiter.Wait();
            Script.Assembly = assemblyAwaiter.Result;

            SceneController.Instance.LoadScene("start.xml", out ScriptInfo[] scripts);
            Script.AddScripts(scripts);

            Script.InitScripts();

            ShaderProgram.ForEachShader(x => x.SetUniform(Window.Scaling, "uProjection"));
            ShaderProgram.ForEachShader(x => x.SetUniform(Matrix4x4f.Identity, "uCameraView"));

            assemblyAwaiter.Dispose();
            GC.Collect();

            while (true)
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
                GraphicsObject.TranslateBuffers("aPos");

                //Render Graphics
                GraphicsObject.Render();

                //Draw colliders as an outline on top of everything else
                //if (_drawColliders) _physicsController.DrawColliders(_sceneController.GlobalTransforms);

                //Update all scripts again
                Script.PostRenderUpdateScripts();

                //Present FrameBuffer
                Window.SwapBuffer();

                Time.StopFrameTimer();
            }
        }

        static void OnKeyDown(object sender, KeyboardState eventArgs)
        {
            if (eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_ESCAPE]) {
                Window.Close();
                GraphicsObject.DisposeAll();
                Environment.Exit(0);
            }
            if (eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_LALT] && eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_KP_ENTER])
            {
                Window.Fullscreen = !Window.Fullscreen;
                Window.CreateWindowAndContext();
            }
        }

        static void OnWindowClose(object sender, EventArgs eventArgs)
        {
            Window.Close();
            GraphicsObject.DisposeAll();
            Environment.Exit(0);
        }

        static void OnWindowSizeChanged(object sender, EventArgs eventArgs)
        {
            Window.UpdateWindowSize();
            ShaderProgram.ForEachShader(x => x.SetUniform(Window.Scaling, "uProjection"));
        }
    }
}
