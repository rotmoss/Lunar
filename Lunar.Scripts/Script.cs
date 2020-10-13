﻿using Lunar.Scene;

namespace Lunar.Scripts
{
    public abstract partial class Script
    {
        public uint Id { get; internal set; }
        public string Name { get; internal set; }
        public float DeltaTime { get => Time.DeltaTime.DeltaTime; }
        public Script GetScript(uint id) { return ScriptController.Instance.GetScript(id); }
        public T[] GetScriptsByType<T>() where T : Script { return ScriptController.Instance.GetScriptsByType<T>(); }
        virtual public void Init() { }
        virtual public void Update() { }
        virtual public void LateUpdate() { }
        virtual public void PostRender() { }
    }
}