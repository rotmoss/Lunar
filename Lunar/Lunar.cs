using OpenGL;
using SDL2;
using System;
using System.Linq;

namespace Lunar
{
    public class Lunar
    {
        private static WindowController _windowController = WindowController.Instance;
        private static SceneController _sceneController = SceneController.Instance;
        private static ScriptController _scriptController = ScriptController.Instance;
        private static GraphicsController _graphicsController = GraphicsController.Instance;
        private static PhysicsController _physicsController = PhysicsController.Instance;
        private static InputController _inputController = InputController.Instance;

        public static bool DrawColliders { get => _drawColliders; set => _drawColliders = value; }
        private static bool _drawColliders;

        static void Main(string[] args)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            _drawColliders = false;

            var assemblyAwaiter = AssemblyCompiler.Instance.CompileScripts();
            _inputController.OnWindowClose += OnWindowClose;
            _inputController.OnWindowSizeChanged += OnWindowSizeChanged;
            _inputController.OnKeyDown += OnKeyDown;

            _windowController.Init();
            _windowController.Fullscreen = false;
            _windowController.Width = 1280;
            _windowController.Height = 720;
            _windowController.CreateWindowAndContext();

            assemblyAwaiter.Wait();
            _scriptController.Assembly = assemblyAwaiter.Result;

            _sceneController.LoadScene("start.xml");

            _scriptController.Init();
            _graphicsController.ForEachShader(x => _graphicsController.SetUniform(x, _windowController.Scaling, "uProjection"));
            _graphicsController.ForEachShader(x => _graphicsController.SetUniform(x, Matrix4x4f.Identity, "uCameraView"));

            assemblyAwaiter.Dispose();
            GC.Collect();

            while (true)
            {
                Time.StartFrameTimer();

                //Invoke Input-Events
                _inputController.PollInputs();

                //Update all scripts
                _scriptController.Update();

                //Update Transforms
                _physicsController.ApplyForces(_sceneController.LocalTransforms);

                //Check Colissions
                _physicsController.CheckColission(_sceneController.GlobalTransforms, _sceneController.LocalTransforms);

                //Use Transfroms to Translate Graphics Data
                _graphicsController.UpdateBuffer(_sceneController.GlobalTransforms);

                //Render Graphics
                _graphicsController.Render();

                //Draw colliders as an outline on top of everything else
                if (_drawColliders) _physicsController.DrawColliders(_sceneController.GlobalTransforms);

                //Update all scripts again
                _scriptController.LateUpdate();

                _scriptController.PostRender();

                //Present FrameBuffer
                _windowController.SwapBuffer();

                Time.StopFrameTimer();
            }
        }

        static void OnKeyDown(object sender, KeyboardState eventArgs)
        {
            if (eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_ESCAPE]) {
                _windowController.Dispose();
                _graphicsController.Dispose();
                Environment.Exit(0);
            }
            if (eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_LALT] && eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_KP_ENTER])
            {
                _windowController.Fullscreen = !_windowController.Fullscreen;
                _windowController.CreateWindowAndContext();
            }
        }

        static void OnWindowClose(object sender, EventArgs eventArgs)
        {
            _windowController.Dispose();
            _graphicsController.Dispose();
            Environment.Exit(0);
        }

        static void OnWindowSizeChanged(object sender, EventArgs eventArgs)
        {
            _windowController.UpdateWindowSize();
            _graphicsController.ForEachShader(x => _graphicsController.SetUniform(x, _windowController.Scaling, "uProjection"));
        }
    }
}
