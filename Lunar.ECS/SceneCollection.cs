using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    public class SceneCollection : ITree<Scene>
    {     
        private List<Scene> _scenes;
        private Dictionary<Guid, Scene> _itemById;
        private Dictionary<string, Scene> _itemByName;

        public int Count { get=> _scenes.Count; }

        public SceneCollection() 
        {
            _scenes = new List<Scene>();
            _itemById = new Dictionary<Guid, Scene>();
            _itemByName = new Dictionary<string, Scene>();
        }

        public void Add(Scene scene, ITreeItem parent = null) 
        {
            scene.Parent = scene;
            scene.Ancestor = scene;

            if(_itemByName.ContainsKey(scene.Name)) 
                scene.Name = Guid.NewGuid().ToString();

            _scenes.Add(scene);
            _itemById.Add(scene.Id, scene);
            _itemByName.Add(scene.Name, scene);
        }

        public void Remove(Scene scene) 
        {
            _scenes.Remove(scene);
            _itemById.Remove(scene.Id);
            _itemByName.Remove(scene.Name);
            scene.Dispose();
        }

        public void Dispose() 
        {
            while (_scenes.Count > 0)
                Remove(_scenes[0]);
        }

        public Scene this[int i] { get => _scenes[i]; }
        public Scene this[Guid i] { get => _itemById.ContainsKey(i) ? _itemById[i] : default; }
        public Scene this[string s] { get => _itemByName.ContainsKey(s) ? _itemByName[s] : default; }
    }
}