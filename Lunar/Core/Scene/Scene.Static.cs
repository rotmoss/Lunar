using System;
using System.Collections.Generic;
using System.Linq;
using Lunar.Graphics;
using Lunar.Scripts;

namespace Lunar.Scenes
{
    public class DisposedEventArgs { public List<uint> Ids; }

    public partial class Scene
    {
        private static IdCollection _idCollection = new IdCollection();

        public static Dictionary<uint, Scene> Scenes { get => _scenes; }
        private static Dictionary<uint, Scene> _scenes = new Dictionary<uint, Scene>();

        private static Dictionary<uint, Scene> _sceneByGameObjectId = new Dictionary<uint, Scene>();
        private static Dictionary<uint, uint> _parentByGameObjectId = new Dictionary<uint, uint>();

        private static Dictionary<uint, string> _nameByGameObjectId = new Dictionary<uint, string>();
        private static Dictionary<string, uint> _gameObjectIdByName = new Dictionary<string, uint>();

        private static Dictionary<string, uint> _sceneIdByName = new Dictionary<string, uint>();

        public EventHandler<DisposedEventArgs> OnSceneDispose;

        public static void LoadScene(string file)
        {
            Scene scene = new Scene();

            scene._gameObjects = new List<uint>();
            scene._id = _idCollection.GetId();
            scene._name = file.Split('.').FirstOrDefault();

            scene.LoadEntites(file);
            _scenes.Add(scene._id, scene);
            _sceneIdByName.Add(scene._name, scene._id);

            scene.OnSceneDispose += GraphicsComponent.OnSceneDispose;
        }

        public static Scene GetSceneFromGameObject(uint id) => _sceneByGameObjectId.ContainsKey(id) ? _sceneByGameObjectId[id] : null;
        public static Scene GetSceneFromGameObject(string name) => GetSceneFromGameObject(_gameObjectIdByName.ContainsKey(name) ? _gameObjectIdByName[name] : 0);
        public static Scene GetScene(uint id) => _scenes.ContainsKey(id) ? _scenes[id] : null;
        public static Scene GetScene(string name) => _sceneIdByName.ContainsKey(name) ? GetScene(_sceneIdByName[name]) : null;

        public static bool IsParent(uint child, uint gameObject)
        {
            if (child == gameObject)
                return true;

            Scene scene = GetSceneFromGameObject(child);
            uint parent = scene.GetParent(child);

            while (parent != 0)
            {
                if (parent == gameObject) return true;
                parent = scene.GetParent(parent);
            }

            return false;
        }
    }
}