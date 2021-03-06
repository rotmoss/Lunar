using System;

namespace Lunar.ECS
{
    public class Gameobject : ITreeItem
    {
        public static GameobjectCollection Collection = new GameobjectCollection();

        public event Action Disposed;

        public ITreeItem Ancestor { get => _ancestor; set => _ancestor = value; }
        private ITreeItem _ancestor;

        public ITreeItem Parent { get => _parent; set => _parent = value; }
        private ITreeItem _parent;

        public Guid Id { get => _id; set => _id = value; }
        private Guid _id;

        public bool Enabled { get => _enabled;  set => _enabled = value; }
        private bool _enabled;

        public string Name { get => _name; set => _name = value; }
        private string _name;

        public Gameobject(string name) 
        {
            _id = Guid.NewGuid();
            _enabled = true;
            _name = name;
            Disposed = null;
        }

        public void Dispose() 
        {
            Disposed?.Invoke();
            _id = Guid.Empty;
            _enabled = false;
            _name = "";
            Disposed = null;
        }
        
        public bool Equals(ITreeItem obj) => obj.GetType() == typeof(Gameobject) && obj.Id == _id;
        
        public Guid[] GetParents() 
        {
            return null;
        }
    }
}