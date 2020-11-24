using Lunar.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public class Gameobject : IDisposable
    {
        public uint Id { get; }

        private static IdCollection _idCollection = new IdCollection();

        private static Dictionary<uint, string> _name = new Dictionary<uint, string>();
        private static Dictionary<uint, uint> _parent = new Dictionary<uint, uint>();
        private static Dictionary<uint, uint> _scene = new Dictionary<uint, uint>();

        public static EventHandler<uint> OnDispose;

        public Gameobject(XmlGameObject xmlGameobject, uint scene)
        {
            Id = _idCollection.GetId();
            _name.Add(Id, xmlGameobject.Name);
            _scene.Add(Id, scene);
            _parent.Add(Id, 0);

            if (xmlGameobject.Components == null) return;

            foreach (XmlComponent component in xmlGameobject.Components)
                component.CreateComponent(Id);
        }

        public void Dispose()
        {
            _name.Remove(Id);
            _parent.Remove(Id);
            _scene.Remove(Id);
            _idCollection.Remove(Id);

            OnDispose.Invoke(this, Id);
        }

        public static string GetName(uint id) => _name.ContainsKey(id) ? _name[id] : "";
        public static uint GetId(string name) => _name.FirstOrDefault(x => x.Value == name).Key;

        public static uint GetScene(uint id) => _scene.ContainsKey(id) ? _scene[id] : 0;
        public static uint GetScene(string name) => GetScene(GetId(name));

        public static uint GetParent(uint id) => _parent.ContainsKey(id) ? _parent[id] : 0;
        public static uint GetParent(string name) => GetParent(GetId(name));

        public static uint[] GetParents(uint id) { List<uint> parents = new List<uint>(); while (id < 0) { id = GetParent(id); parents.Add(id); } return parents.ToArray(); }
        public static uint[] GetParents(string name) { List<uint> parents = new List<uint>(); uint id = GetId(name); while (id < 0) { id = GetParent(id); parents.Add(id); } return parents.ToArray(); }

        public static uint[] GetChildren(uint id) => _parent.Where(x => x.Value == id).Select(x => x.Key).ToArray();
        public static uint[] GetChildren(string name) => GetChildren(GetId(name));

        public static void SetParent(uint id, uint value) { if (!_parent.ContainsKey(id)) return; _parent[id] = value; }

        public static bool IsParent(uint child, uint gameObject)
        {
            if (child == gameObject)
                return true;

            uint parent = GetParent(child);

            while (parent != 0) {
                if (parent == gameObject) return true;
                parent = GetParent(parent);
            }

            return false;
        }
    }
}
