using Lunar.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.ECS
{
    public class HierarchyTree
    {
        public Dictionary<uint, uint> _parent;

        public HierarchyTree()
        {
            _parent = new Dictionary<uint, uint>();
        }

        public uint GetParent(uint id) => _parent.ContainsKey(id) ? _parent[id] : 0;
        internal void SetParent(uint id, uint value) { if (!_parent.ContainsKey(id)) return; _parent[id] = value; }

        public List<uint> GetParents(uint id) { List<uint> parents = new List<uint>(); while (id < 0) { id = GetParent(id); parents.Add(id); } return parents; }
        public List<uint> GetChildren(uint id) => _parent.Where(x => x.Value == id).Select(x => x.Key).ToList();

        public bool IsParent(uint child, uint id)
        {
            if (child == id)
                return true;

            uint parent = GetParent(child);

            while (parent != 0)
            {
                if (parent == id) return true;
                parent = GetParent(parent);
            }

            return false;
        }
    }

    public class Gameobject : ObjectIdentifier<Gameobject>
    {
        public static HierarchyTree HierarchyTree = new HierarchyTree();

        public string Name { get => _name; set => _name = value; }
        private string _name;

        public Gameobject(XmlGameObject xmlGameobject) : base()
        {
            _name = xmlGameobject.Name;
            Enabled = xmlGameobject.Enabled;

            foreach(XmlComponent component in xmlGameobject.Components)
                component.CreateComponent(this);
        }

        public static string GetName(uint id) => Collection.GetEntryById(id).Name;
        public static uint GetId(string name) => Collection.Entries.Where(x => x.Name == name).FirstOrDefault().Id;

        public static uint GetParent(uint id) => HierarchyTree.GetParent(id);
        public static uint GetParent(string name) => GetParent(GetId(name));

        public static void SetParent(string child, string parent) => HierarchyTree.SetParent(GetId(child), GetId(parent));

        public override void DerivedDispose() { }
    }
}
