﻿using System;

namespace Lunar
{
    public abstract class Script
    {
        public uint _id;
        public string _name;

        public Transform _transform 
        { 
            get { return SceneController.GetEntityTransform(_id); } 
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
