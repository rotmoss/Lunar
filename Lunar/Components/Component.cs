using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public abstract class Component<T> : IDisposable where T : Component<T>
    {
        protected static Dictionary<uint, List<T>> _dictionary = new Dictionary<uint, List<T>>();
        protected static List<T> _components = new List<T>();

        public uint Id { get => _id; private set => _id = value; }
        protected uint _id;

        public bool Enabled { get => _enabled; set => _enabled = value; }
        protected bool _enabled { get; set; }

        public Component()
        {
            _id = 0;
            _enabled = true;
        }

        public abstract void DisposeChild();

        public void Dispose()
        {
            DisposeChild();
            _dictionary[_id].Remove((T)this);
            _components.Remove((T)this);
        }

        public static void Close()
        {
            while(_components.Count > 0)
                _components[^1].Dispose();
            _dictionary.Clear();
        }

        public static void OnGameobjectDispose(object sender, uint id)
        {
            while(_dictionary[id].Count > 0)
                _dictionary[id][^1].Dispose();
        }

        public static List<T> GetComponents(uint id) 
        {
           return _dictionary.ContainsKey(id) ? _dictionary[id] : default;
        }

        public static T GetComponent(uint id)
        {
            return _dictionary.ContainsKey(id) ? _dictionary[id].FirstOrDefault() : default;
        }

        public static void AddComponent(uint id, T component)
        {
            component._id = id;
            if (!_dictionary.ContainsKey(id)) _dictionary.Add(id, new List<T>());
            _dictionary[id].Add(component);

            _components.Add(component);
        }
    }
}
