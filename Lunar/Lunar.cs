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
            _inputController.WindowChange += OnWindowChange;
            _inputController.KeyDown += OnKeyDown;

            _windowController.Init();
            _windowController.Fullscreen = false;
            _windowController.Width = 960;
            _windowController.Height = 540;
            _windowController.CreateWindowAndContext();

            assemblyAwaiter.Wait();
            _scriptController.Assembly = assemblyAwaiter.Result;

            _sceneController.LoadScene("start.ini");

            _scriptController.Init();
            _graphicsController.ForeachShader(x => _graphicsController.SetUniform(x, _windowController.Scaling, "uProjection"));

            assemblyAwaiter.Dispose();
            GC.Collect();

            while (true)
            {
                Time.StartFrameTimer();

                //Invoke Input-Events
                _inputController.InvokeInputEvents(null);

                //Update all scripts
                _scriptController.Update();

                Dictionary<uint, Transform> transforms = _sceneController.Transforms;

                //Update Transforms
                _physicsController.ApplyForces(transforms);

                //Check Colissions
                _physicsController.CheckColission(transforms);

                //Use Transfroms to Translate Graphics Data
                _graphicsController.TranslateBuffers(transforms);

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

        static void OnKeyDown(object sender, KeyboardEventArgs eventArgs) 
        {
            if(eventArgs.EventTypes.Contains(KeyEvent.ESC))
            {
                _windowController.Dispose();
                _graphicsController.Dispose();
                Environment.Exit(0);
            }
            if (eventArgs.EventTypes.Contains(KeyEvent.ENTER) && eventArgs.EventTypes.Contains(KeyEvent.LCTRL))
            {
                _windowController.Fullscreen = !_windowController.Fullscreen;
                _windowController.CreateWindowAndContext();
            }
        }

        static void OnWindowChange(object sender, WindowEventArgs eventArgs)
        {
            if(eventArgs.EventTypes.Contains(WindowEvent.EXIT))
            { 
                _windowController.Dispose();
                _graphicsController.Dispose();
                Environment.Exit(0); 
            }
            if(eventArgs.EventTypes.Contains(WindowEvent.RESIZE))
            {
                _windowController.UpdateWindowSize();
                _graphicsController.ForeachShader(x => _graphicsController.SetUniform(x, _windowController.Scaling, "uProjection"));
            }
        }
    }
}
