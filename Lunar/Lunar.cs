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
    class Lunar
    {
        private static GLContext _context;
        private static SceneController _sceneController = SceneController.Instance;
        private static ScriptController _scriptController = ScriptController.Instance;
        private static GraphicsController _graphicsController = GraphicsController.Instance;
        private static PhysicsController _physicsController = PhysicsController.Instance;
        private static InputController _inputController = InputController.Instance;
        private static Dictionary<uint, Transform> _transforms;  

        static void Main(string[] args)
        {
            var assemblyAwaiter = AssemblyCompiler.Instance.CompileScripts();
            _inputController.WindowChange += OnWindowChange;

            _context = new GLContext(1920, 1080, false);
            _context.Init();

            assemblyAwaiter.Wait();
            _scriptController.Assembly = assemblyAwaiter.Result;

            _sceneController.LoadScene("start.ini");

            _scriptController.InitScripts();
            _graphicsController.ForeachShader(x => _graphicsController.SetUniform(x, _context.Scaling, "uProjection"));

            assemblyAwaiter.Dispose();
            GC.Collect();

            while (true)
            {
                Time.StartFrameTimer();

                //Invoke Input-Events
                _inputController.InvokeInputEvents(null);

                //Update all scripts
                _scriptController.UpdateScripts();

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
                _physicsController.DrawColliders(transforms);

                //Update all scripts again
                _scriptController.LateUpdateScripts();

                //Send DeltaTime to scripts
                _scriptController.UpdateDeltaTime(Time.DeltaTime);

                //Present FrameBuffer
                _context.SwapBuffer();

                Time.StopFrameTimer();
            }
        }

        static void OnWindowChange(object sender, WindowEventArgs eventArgs)
        {
            if(eventArgs.EventTypes.Contains(WindowEvent.EXIT))
            { 
                _context.Dispose();
                _graphicsController.Dispose();
                Environment.Exit(0); 
            }
            if(eventArgs.EventTypes.Contains(WindowEvent.RESIZE))
            {
                _context.UpdateWindowSize();
                _graphicsController.ForeachShader(x => _graphicsController.SetUniform(x, _context.Scaling, "uProjection"));
            }
        }
    }
}
