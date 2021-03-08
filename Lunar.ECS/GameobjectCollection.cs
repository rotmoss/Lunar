using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    public class GameobjectCollection : ITree<Gameobject>
    {    
        private List<Gameobject> _gameobjects;
        private Dictionary<Guid, Gameobject> _itemById;
        private Dictionary<string, Gameobject> _itemByName;

        public int Count { get => _gameobjects.Count; }

        public GameobjectCollection() 
        {
            _itemById = new Dictionary<Guid, Gameobject>();
            _itemByName = new Dictionary<string, Gameobject>();
            _gameobjects = new List<Gameobject>();
        }

        public void Add(Gameobject gameobject, ITreeItem parent) 
        {
            gameobject.Parent = parent;

            gameobject.Ancestor = parent.Ancestor;

            if(_itemByName.ContainsKey(gameobject.Name)) 
                gameobject.Name = Guid.NewGuid().ToString();

            _gameobjects.Add(gameobject);
            _itemById.Add(gameobject.Id, gameobject);
            _itemByName.Add(gameobject.Name, gameobject);
        }

        public void Remove(Gameobject gameobject) 
        {
            _itemById.Remove(gameobject.Id);
            _itemByName.Remove(gameobject.Name);
            _gameobjects.Remove(gameobject);
            gameobject.Dispose();
        }

        public Gameobject this[int i] { get => _gameobjects[i]; }
        public Gameobject this[Guid i] { get => _itemById.ContainsKey(i) ? _itemById[i] : default; }
        public Gameobject this[string s] { get => _itemByName.ContainsKey(s) ? _itemByName[s] : default; }
    }
}