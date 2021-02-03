using System;
using System.Threading.Tasks;
using Lunar.Input;
using Lunar.ECS;
using Lunar.SDL;
using Lunar.GL;
using Lunar.Compiler;
using System.Reflection;
using Lunar.ECS.Components;
using System.Collections.Generic;

namespace Lunar
{ 
    public class LunarEngine
    {
        public Queue<Action> Actions { get => _actions; }
        private Queue<Action> _actions;

        public bool Pause { get => _pause; }
        private bool _pause;

        public LunarEngine(IRenderBehaviour renderBehaviour, IWindowContext windowContext)
        {
            Task<Assembly> assembly = AssemblyCompiler.CompileScripts();

            Window.Init(windowContext, renderBehaviour);

            InputController.Init();
            InputController.OnWindowClose += OnWindowClose;

            assembly.Wait();
            Script.Assembly = assembly.Result;

            Scene.Collection.AddEntry(null, new Scene("start.xml"));

            Script.InitScripts();
            Script.LateInitScripts();

            _actions = new Queue<Action>();
        }

        public void Update()
        {
            //Update all scripts
            Script.UpdateScripts();

            //Update Transforms
            Force.ApplyForces();

            //Check Colissions
            Collider.CheckColissions();

            //Update all scripts again
            Script.LateUpdateScripts();

            Animation.Animate(Time.FrameTime);
        }

        public void Render()
        {
            //Use Transfroms to Translate Graphics Data
            GraphicsComponent.TranslateBuffers();

            //Render Graphics
            Window.RenderBehaviour.Render();

            //Update all scripts again
            Script.PostRenderUpdateScripts();

            //Present FrameBuffer
            Window.SwapBuffer();
        }

        public void NextAction()
        {
            if (_actions.TryDequeue(out Action action))
                action.Invoke();
        }

        static void OnWindowClose(object sender, EventArgs eventArgs)
        {
            Scene.Collection.Dispose();
            Window.Dispose();
            Mixer.Dispose();
            Environment.Exit(0);
        }
    }
}
