using Lunar.Scripts;

namespace Lunar
{
    public abstract class ScriptTemplate
    {
        internal Script Container;

        public uint Id { get => Container.Id; }
        public bool Enabled { get => Container.Enabled; }

        public string Name { get => Gameobject.GetName(Id); }
        public uint Parent { get => Gameobject.GetParent(Id); set => Gameobject.SetParent(Id, value); }
        public uint Scene { get => Gameobject.GetScene(Id); }

        public virtual void Init()       { }
        public virtual void LateInit()   { }
        public virtual void Update()     { }
        public virtual void LateUpdate() { }
        public virtual void PostRender() { }

        public Transform Transform { get => Transform.GetGlobalTransform(Id); set => Transform.SetTransform(Id, value); }
        public Transform LocalTransform { get => Transform.GetLocalTransform(Id); set => Transform.SetTransform(Id, value); }
    }
}
