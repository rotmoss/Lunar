using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    public class Component<T> : ITreeItem where T : Component<T>
    {
        public static ComponentCollection<T> Collection = new ComponentCollection<T>();
       
        public event Action Disposed;

        public ITreeItem Ancestor { get => _ancestor; set => _ancestor = value; }
        private ITreeItem _ancestor;

        public ITreeItem Parent { get => _parent;  set => _parent = value; }
        private ITreeItem _parent;
  
        public Guid Id { get => _parent.Id;  set => _parent.Id = value; }

        public bool Enabled { get => _enabled;  set => _enabled = value; }
        private bool _enabled;

        public string Name { get => _parent.Name;  set => _parent.Name = value; }

        public Component() 
        {
            _enabled = true;
            Disposed = null;
        }

        public void Dispose() 
        {
            Disposed?.Invoke();
            _enabled = false;
            Disposed = null;
        }
        
        public bool Equals(ITreeItem obj) => obj.GetType() == typeof(Component<T>) && obj.Id == Id;
    }
}