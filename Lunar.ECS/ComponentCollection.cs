using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.ECS
{   
    public class ComponentCollection<T> : ITree<T> where T : Component<T>
    {
        private List<T> _components;
        private Dictionary<Guid, T> _itemById;
        private Dictionary<string, T> _itemByName;

        public int Count { get => _components.Count; }
        public List<T>.Enumerator GetEnumerator() => _components.GetEnumerator();

        public ComponentCollection() 
        {
            _components = new List<T>();
            _itemById = new Dictionary<Guid, T>();
            _itemByName = new Dictionary<string, T>();
        }

        public void Add(T component, ITreeItem parent) 
        {
            component.Parent = parent;     
            component.Ancestor = parent.Ancestor;
            parent.Disposed += component.Dispose;
            parent.Disposed += () => Remove(component);

            _components.Add(component);
            _itemById.Add(component.Id, component);
            _itemByName.Add(component.Name, component);
        }

        public void Remove(T component) 
        {
            _components.Remove(component);
            _itemById.Remove(component.Id);
            _itemByName.Remove(component.Name);
        }

        public T this[int i] { get => _components[i]; }
        public T this[Guid i] { get => _itemById.ContainsKey(i) ? _itemById[i] : default; }
        public T this[string s] { get => _itemByName.ContainsKey(s) ? _itemByName[s] : default; }
    }
}
