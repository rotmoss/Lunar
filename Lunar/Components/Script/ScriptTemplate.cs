using Lunar.Scenes;
using Lunar.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public abstract class ScriptTemplate
    {
        internal Script _container;

        public uint Id { get => _container.Id; }
        public bool Enabled { get => _container.Enabled; }

        public string Name { get => Scene.GetGameObjectName(Id); set => Scene.SetName(Id, value); }
        public uint Parent { get => Scene.GetParent(Id); set => Scene.SetParent(Id, value); }
        public Scene Scene { get => Scene.GetSceneFromGameObject(Id); }

        public virtual void Init() { }
        public virtual void LateInit() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void PostRender() { }

        public Transform Transform { get => Transform.GetGlobalTransform(Id); set => Transform.SetTransform(Id, value); }
        public Transform LocalTransform { get => Transform.GetLocalTransform(Id); set => Transform.SetTransform(Id, value); }
    }
}
