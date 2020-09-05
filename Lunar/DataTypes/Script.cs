using System;

namespace Lunar
{
    public abstract partial class Script
    {
        public uint _id;
        public string _name;

        public Transform _localTransform 
        { 
            get { return SceneController.GetEntityLocalTransform(_id); } 
            set { SceneController.SetEntityTransform(_id, value); } 
        }

        public Transform _globalTransform
        {
            get { return SceneController.GetEntityGlobalTransform(_id); }
            set { SceneController.SetEntityTransform(_id, value); }
        }
        public bool _visible 
        { 
            get { return SceneController.GetEntityVisibility(_id); } 
            set { SceneController.SetEntityVisibility(_id, value); } 
        }

        protected GraphicsController GraphicsController;
        protected SceneController SceneController;
        protected ScriptController ScriptController;
        protected PhysicsController PhysicsController;
        protected InputController InputController;

        public float DeltaTime;

        public Script()
        {
            GraphicsController = GraphicsController.Instance;
            SceneController = SceneController.Instance;
            ScriptController = ScriptController.Instance;
            InputController = InputController.Instance;
            PhysicsController = PhysicsController.Instance;
        }

        virtual public void Init() { }
        virtual public void Update() { }
        virtual public void LateUpdate() { }
        virtual public void PostRender() { }
    }
}
