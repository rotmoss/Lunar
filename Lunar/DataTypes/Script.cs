using System;

namespace Lunar
{
    public abstract class Script
    {
        public uint _id;
        public bool _render;
        public string _name;

        public Transform _transform;

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

        abstract public void Init();
        abstract public void Update();
        abstract public void LateUpdate();
    }
}
