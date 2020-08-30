using OpenGL;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        static void Main(string[] args)
        {
            var assemblyAwaiter = AssemblyCompiler.Instance.CompileScripts();
            _inputController.OnWindowClose += OnWindowClose;
            _inputController.OnWindowResized += OnWindowResized;
            _inputController.OnKeyDown += OnKeyDown;

            _windowController.Init();
            _windowController.Fullscreen = false;
            _windowController.Width = 960;
            _windowController.Height = 540;
            _windowController.CreateWindowAndContext();

            assemblyAwaiter.Wait();
            _scriptController.Assembly = assemblyAwaiter.Result;

            _sceneController.LoadScene("start.xml");

            _scriptController.Init();
            _graphicsController.ForeachShader(x => _graphicsController.SetUniform(x, _windowController.Scaling, "uProjection"));

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
                _graphicsController.TranslateBuffers(_sceneController.GlobalTransforms);

                //Render Graphics
                _graphicsController.Render(_sceneController.Visible.Where(x => x.Value).Select(x => x.Key).ToList());

                //Draw colliders as an outline on top of everything else
                //_physicsController.DrawColliders(transforms);

                //Update all scripts again
                _scriptController.LateUpdate();

                //Send DeltaTime to scripts
                _scriptController.UpdateDeltaTime(Time.DeltaTime);

                _scriptController.PostRender();

                //Present FrameBuffer
                _windowController.SwapBuffer();

                Time.StopFrameTimer();
            }
        }

        static void OnKeyDown(object sender, KeyboardState eventArgs)
        {
            if (eventArgs.RawStates[SDL.SDL_Keycode.SDLK_ESCAPE]) {
                _windowController.Dispose();
                _graphicsController.Dispose();
                Environment.Exit(0);
            }
            if (eventArgs.RawStates[SDL.SDL_Keycode.SDLK_LALT] && eventArgs.RawStates[SDL.SDL_Keycode.SDLK_KP_ENTER])
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

        static void OnWindowResized(object sender, EventArgs eventArgs)
        {
            _windowController.UpdateWindowSize();
            _graphicsController.ForeachShader(x => _graphicsController.SetUniform(x, _windowController.Scaling, "uProjection"));
        }
    }
}
