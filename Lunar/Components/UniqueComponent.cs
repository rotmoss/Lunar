using Lunar.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public abstract class UniqueComponent<T> where T : UniqueComponent<T>
    {
        protected static Dictionary<uint, T> _components = new Dictionary<uint, T>();

        public uint Id { get => _id; }
        protected uint _id;

        public bool Enabled { get; set; }
        protected bool _enabled { get; set; }

        public UniqueComponent()
        {
            _id = 0;
            _enabled = true;
        }

        public abstract void DisposeChild();

        public void Dispose()
        {
            DisposeChild();
            _components.Remove(_id);
        }

        public static void Close()
        {
            while(_components.Count > 0)
                _components.Last().Value.Dispose();
        }

        public static void OnSceneDispose(object sender, DisposedEventArgs e)
        {
            foreach (uint key in e.Ids)
                    _components[key].Dispose();
        }

        public static void OnGameObjectDispose(object sender, DisposedEventArgs e)
        {
            foreach (uint key in e.Ids)
                _components[key].Dispose();
        }

        public static T GetComponent(uint id) 
        {
           return _components.ContainsKey(id) ? _components[id] : default;
        }

        public static void AddComponent(uint id, T component)
        {
            component._id = id;
            if (!_components.ContainsKey(id)) _components.Remove(id);
            _components.Add(id, component);
        }
    }
}
