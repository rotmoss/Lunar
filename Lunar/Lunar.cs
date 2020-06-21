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
        private static InputController _inputController = InputController.Instance;

        static void Main(string[] args)
        {
            var assemblyAwaiter = AssemblyCompiler.Instance.CompileScripts();
            _inputController.WindowChange += OnWindowChange;

            _context = new GLContext(640, 480, false);
            _context.Init();

            assemblyAwaiter.Wait();
            _scriptController.Assembly = assemblyAwaiter.Result;

            _sceneController.LoadScene("start.ini");

            _scriptController.InitScripts();
            _graphicsController.ForeachShader(x => _graphicsController.SetUniform(x, _context.Scaling, "uProjection"));

            while (true)
            {
                Time.StartFrameTimer();
                _inputController.InvokeInputEvents(null);

                _scriptController.UpdateScripts();
                _scriptController.LateUpdateScripts();

                _graphicsController.TranslateBuffers(_scriptController.GetEntityTransforms());
                _graphicsController.Render(_scriptController.GetRenderQueue());

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
