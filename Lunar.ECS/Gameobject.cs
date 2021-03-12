using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    public class Gameobject : ICollectionItem
    {
        public static GameobjectCollection Collection = new GameobjectCollection();

        public event Action Disposed;

        public ICollectionItem Ancestor { get => _ancestor; set => _ancestor = value; }
        private ICollectionItem _ancestor;

        public ICollectionItem Parent { get => _parent; set => _parent = value; }
        private ICollectionItem _parent;

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
        
        public bool Equals(ICollectionItem obj) => obj.GetType() == typeof(Gameobject) && obj.Id == _id;
        
        public List<Guid> GetParents() 
        {
            List<Guid> parents = new List<Guid>();
            ICollectionItem current = _parent;

            while (current.GetType() == typeof(Gameobject)) 
            {
                parents.Add(current.Id);
                current = current.Parent;
            }

            return parents;
        }
    }
}