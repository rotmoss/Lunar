using System;
using System.Collections.Generic;
using Lunar.IO;
using System.IO;
using System.Linq;
using Lunar.Scripts;
using Lunar.Graphics;
using System.Reflection;

namespace Lunar.Scenes
{
    public partial class Scene
    {
        public uint Id { get => _id; }
        private uint _id;

        public string Name { get => _name; }
        private string _name;

        public List<uint> GameObjects { get => _gameObjects; }
        private List<uint> _gameObjects;

        internal void LoadEntites(string file)
        {
            if (!File.Exists(FileManager.Path + "Scenes" + FileManager.Seperator + file)) { 
                Console.WriteLine("Could not find scene " + file);  
                return; 
            }

            XmlScene scene = FileManager.Dezerialize(file, "Scenes", "scene");

            foreach (XmlGameObject entity in scene.entities)
            {
                uint gameObject = _idCollection.GetId();
                _gameObjects.Add(gameObject);

                _nameByGameObjectId.Add(gameObject, entity.Name.ToLower());
                _gameObjectIdByName.Add(entity.Name.ToLower(), gameObject);
                _sceneByGameObjectId.Add(gameObject, this);

                if (entity.Components != null) {
                    foreach (XmlComponent component in entity.Components)
                        component.CreateComponent(gameObject);               
                }
            }

            foreach (XmlGameObject entity in scene.entities)
            {
                if (string.IsNullOrEmpty(entity.Parent)) continue;

                uint id = _gameObjectIdByName[entity.Name];
                uint parentId = _gameObjectIdByName[entity.Parent];

                _parentByGameObjectId.Add(id, parentId);
            }
        }

        public void Dispose()
        {
            OnSceneDispose.Invoke(this, new DisposedEventArgs { Ids = _gameObjects });

            foreach(uint id in _gameObjects)
            {
                _gameObjectIdByName.Remove(_nameByGameObjectId[id]);
                _nameByGameObjectId.Remove(id);
                _parentByGameObjectId.Remove(id);
                _sceneByGameObjectId.Remove(id);
                _idCollection.Remove(id);
            }

            _gameObjects.Clear();
            _idCollection.Remove(_id);
        }

        public void DestroyGameObject(uint id) { GetSceneFromGameObject(id)._gameObjects.Remove(id); OnSceneDispose(this, new DisposedEventArgs { Ids = new List<uint>(GetChildren(id).Concat(new uint[] { id })) }); }
        
        public string GetGameObjectName(uint id) => _nameByGameObjectId.ContainsKey(id) ? _gameObjects.Contains(id) ? _nameByGameObjectId[id] : "" : "";
        public uint GetGameObjectId(string name) => _gameObjectIdByName.ContainsKey(name.ToLower()) ? _gameObjects.Contains(_gameObjectIdByName[name.ToLower()]) ? _gameObjectIdByName[name.ToLower()] : 0: 0;
       
        public uint GetParent(uint id) => _parentByGameObjectId.ContainsKey(id) ? _gameObjects.Contains(id) ? _parentByGameObjectId[id] : 0 : 0;
        public uint GetParent(string name) => GetParent(GetGameObjectId(name));
        
        public uint[] GetParents(uint id) { List<uint> parents = new List<uint>(); while (id < 0) { id = GetParent(id); parents.Add(id); } return parents.ToArray(); }
        public uint[] GetParents(string name) { List<uint> parents = new List<uint>(); uint id = GetGameObjectId(name); while (id < 0) { id = GetParent(id); parents.Add(id); } return parents.ToArray(); }
        
        public uint[] GetChildren(uint id) => _parentByGameObjectId.Keys.Where(x => _parentByGameObjectId[x] == id).ToArray();
        public uint[] GetChildren(string name) => GetChildren(GetGameObjectId(name));

        public void SetName(uint id, string value) { if (_gameObjects.Contains(id) && _nameByGameObjectId.ContainsKey(id)) _nameByGameObjectId[id] = value; }
        public void SetParent(uint id, uint value) { if (_gameObjects.Contains(id) && _nameByGameObjectId.ContainsKey(id)) _parentByGameObjectId[id] = value; }
    }
}