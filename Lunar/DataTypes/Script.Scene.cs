using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public partial class Script
    {
        public Transform LocalTransform { get => SceneController.Instance.GetEntityLocalTransform(Id); set => SceneController.Instance.SetEntityTransform(Id, value); }
        public Transform GlobalTransform { get => SceneController.Instance.GetEntityGlobalTransform(Id); set => SceneController.Instance.SetEntityTransform(Id, value); }
        public Transform GetGlobalTransform(uint id) => SceneController.Instance.GetEntityGlobalTransform(id);
        public Transform GetLocalTransform(uint id) => SceneController.Instance.GetEntityLocalTransform(id);
        public uint Parent { get => SceneController.Instance.GetEntityParent(Id); set => SceneController.Instance.SetEntityParent(Id, value); }
        public uint GetEntityId(string name) => SceneController.Instance.GetEntityID(name);
        public string GetEntityName(uint id) => SceneController.Instance.GetEntityName(id);
    }
}
