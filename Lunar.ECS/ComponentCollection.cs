using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.ECS
{   
    public class ComponentCollection<T> : ITree<T> where T : Component<T>
    {
        private Dictionary<Guid, T> _itemById;
        private Dictionary<string, T> _itemByName;

        public ComponentCollection() 
        {
            _itemById = new Dictionary<Guid, T>();
            _itemByName = new Dictionary<string, T>();
        }

        public void Add(T component, ITreeItem parent) 
        {
            component.Parent = parent;     
            component.Ancestor = parent.Ancestor;
            parent.Disposed += component.Dispose;

            _itemById.Add(component.Id, component);
            _itemByName.Add(component.Name, component);
        }

        public void Remove(T component) 
        {
            _itemById.Remove(component.Id);
            _itemByName.Remove(component.Name);
        }

        public T this[Guid i] { get => _itemById.ContainsKey(i) ? _itemById[i] : default; }
        public T this[string s] { get => _itemByName.ContainsKey(s) ? _itemByName[s] : default; }
    }
}
