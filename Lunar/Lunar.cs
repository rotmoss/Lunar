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
        private static Context _context;

        static void Main(string[] args)
        {
            var assemblyAwaiter = AssemblyCompiler.Instance.CompileScripts();
            InputManager.WindowChange += OnWindowChange;

            _context = new Context(640, 480, false);
            _context.Init();

            assemblyAwaiter.Wait();
            ScriptController.Instance.Assembly = assemblyAwaiter.Result;

            SceneController.Instance.LoadScene("start.ini");

            ScriptController.Instance.InitScripts();
            Graphics.Instance.ForeachShader(x => Graphics.Instance.SetUniform(x, _context.Scaling, "uProjection"));

            while (true)
            {
                InputManager.InvokeInputEvents(null);

                ScriptController.Instance.UpdateScripts();
                ScriptController.Instance.LateUpdateScripts();

                Graphics.Instance.UpdateMatrix(ScriptController.Instance.GetTransforms());
                Graphics.Instance.Render(ScriptController.Instance.GetRenderQueue());

                _context.SwapBuffer();
            }
        }

        static void OnWindowChange(object sender, WindowEventArgs eventArgs)
        {
            if(eventArgs.EventTypes.Contains(WindowEvent.EXIT))
            { 
                _context.Dispose();
                Graphics.Instance.Dispose();
                Environment.Exit(0); 
            }
            if(eventArgs.EventTypes.Contains(WindowEvent.RESIZE))
            {
                _context.UpdateWindowSize();
                Graphics.Instance.ForeachShader(x => Graphics.Instance.SetUniform(x, _context.Scaling, "uProjection"));
            }
        }
    }
}
