namespace Lunar.Scene
{
    public partial class SceneController
    {
        public uint GetEntityID(string name)
        {
            foreach (uint id in _names.Keys)
                if (_names[id].ToLower() == name.ToLower()) return id;
            return 0;
        }
        public string GetEntityName(uint id)
        {
            if (_names.ContainsKey(id)) return _names[id];
            return null;
        }
        public uint GetEntityParent(uint id) => _parent.ContainsKey(id) ? _parent[id] : 0;
        public void SetEntityParent(uint id, uint parent) 
        {
            if (!_parent.ContainsKey(id)) _parent.Add(id, parent);
            else _parent[id] = parent; 
        }
    }
}
