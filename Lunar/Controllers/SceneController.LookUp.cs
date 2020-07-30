namespace Lunar
{
    partial class SceneController
    {
        public uint GetEntityID(string name)
        {
            foreach (uint id in _name.Keys)
                if (_name[id] == name) return id;
            return 0;
        }
        public uint GetEntityParent(uint id) => _parent.ContainsKey(id) ? _parent[id] : 0;
        public void SetEntityParent(uint id, uint parent) 
        {
            if (!_parent.ContainsKey(id)) _parent.Add(id, parent);
            else _parent[id] = parent; 
        }
    }
}
