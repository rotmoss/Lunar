using Lunar.Scene;
using Lunar.Math;

namespace Lunar.Scripts
{
    public partial class Script
    {
        public Transform Transform { get => Transform.GetTransform(Id); set => Transform.SetTransform(Id, value); }
        public uint Parent { get => SceneController.Instance.GetEntityParent(Id); set => SceneController.Instance.SetEntityParent(Id, value); }
        public uint GetEntityId(string name) => SceneController.Instance.GetEntityID(name);
        public string GetEntityName(uint id) => SceneController.Instance.GetEntityName(id);
    }
}
