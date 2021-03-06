using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    public class SceneCollection : ITree<Scene>
    {     
        public Dictionary<Guid, Scene> _itemById = new Dictionary<Guid, Scene>();
        public Dictionary<string, Scene> _itemByName = new Dictionary<string, Scene>();

        public SceneCollection() 
        {
            _itemById = new Dictionary<Guid, Scene>();
            _itemByName = new Dictionary<string, Scene>();
        }

        public void Add(Scene scene, ITreeItem parent = null) 
        {
            scene.Parent = scene;
            scene.Ancestor = scene;

            if(_itemByName.ContainsKey(scene.Name)) 
                scene.Name = Guid.NewGuid().ToString();

            _itemById.Add(scene.Id, scene);
            _itemByName.Add(scene.Name, scene);
        }

        public void Remove(Scene scene) 
        {
            _itemById.Remove(scene.Id);
            _itemByName.Remove(scene.Name);
            scene.Dispose();
        }

        public Scene this[Guid i] { get => _itemById.ContainsKey(i) ? _itemById[i] : default; }
        public Scene this[string s] { get => _itemByName.ContainsKey(s) ? _itemByName[s] : default; }
    }
}