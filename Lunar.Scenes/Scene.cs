using System;
using System.Collections.Generic;
using Lunar.IO;
using System.IO;
using System.Linq;

namespace Lunar.Scenes
{
    public struct ScriptInfo
    {
        public uint id;
        public (string, string)[] vars;
        public string className;
    }

    public class Scene
    {
        private List<uint> _gameObjects;
        private uint id;

        private static Dictionary<uint, Scene> SceneByID;
        private static Dictionary<uint, uint> ParentByID;
        private static Dictionary<uint, string> NameByID;
        private static Dictionary<string, uint> IDByName;
        private static IdCollection IDs;
        private static List<Scene> Scenes;

        private Scene()
        {
            IDs = new IdCollection();

            NameByID = new Dictionary<uint, string>();
            ParentByID = new Dictionary<uint, uint>();
            SceneByID = new Dictionary<uint, Scene>();
            IDByName = new Dictionary<string, uint>();
            Scenes = new List<Scene>();
            _gameObjects = new List<uint>();
    }

        public static void LoadScene(string file, out ScriptInfo[] scripts)
        {
            Scene scene = new Scene();

            scene.id = IDs.GetId();

            if (!NameByID.ContainsKey(scene.id)) NameByID.Add(scene.id, file);
            else { NameByID[scene.id] = file; }

            scene.LoadEntites(file, out scripts);
            Scenes.Add(scene);
        }

        internal void LoadEntites(string file, out ScriptInfo[] scripts)
        {
            if (!File.Exists(FileManager.Path + "Scenes" + FileManager.Seperator + file))
            { Console.WriteLine("Could not find scene " + file); scripts = null; return; }

            XmlElementEntity[] array = FileManager.Dezerialize<XmlElementScene>(file, "Scenes", "scene").entities;
            List<ScriptInfo> scriptList = new List<ScriptInfo>();

            foreach (XmlElementEntity entity in array)
            {
                uint gameObject = IDs.GetId();

                NameByID.Add(gameObject, entity.Name.ToLower());
                IDByName.Add(entity.Name.ToLower(), gameObject);
                SceneByID.Add(gameObject, this);

                Transform.AddTransform(gameObject);

                List<(string, string)> variables = new List<(string, string)>();
                if (entity.Script.Vars != null)
                    foreach (XmlElementVar var in entity.Script.Vars) variables.Add((var.Name, var.Value));
                scriptList.Add(new ScriptInfo { id = gameObject, className = entity.Script.File, vars = variables.ToArray() });

                _gameObjects.Add(gameObject);
            }

            scripts = scriptList.ToArray();

            foreach (XmlElementEntity entity in array)
            {
                if (string.IsNullOrEmpty(entity.Parent)) continue;

                uint id = IDByName[entity.Name];
                uint parentId = IDByName[entity.Parent];

                ParentByID.Add(id, parentId);
            }
        }

        public void Dispose()
        {
            foreach(uint id in _gameObjects)
            {
                IDByName.Remove(NameByID[id]);
                NameByID.Remove(id);
                ParentByID.Remove(id);
                SceneByID.Remove(id);
                IDs.Remove(id);
            }

            _gameObjects.Clear();
            IDs.Remove(id);
        }

        public static void DisposeAll()
        {
            foreach (Scene scene in Scenes)
                scene.Dispose();
        }

        public string GetName(uint id) => NameByID.ContainsKey(id) ? _gameObjects.Contains(id) ? NameByID[id] : "" : "";
        public uint GetParent(uint id) => ParentByID.ContainsKey(id) ? _gameObjects.Contains(id) ? ParentByID[id] : 0 : 0;
        public uint GetId(string name) =>  IDByName.ContainsKey(name.ToLower()) ? _gameObjects.Contains(IDByName[name.ToLower()]) ? IDByName[name.ToLower()] : 0: 0;
        public uint GetParent(string name) => GetParent(GetId(name));

        public void SetName(uint id, string value) { if (_gameObjects.Contains(id) && NameByID.ContainsKey(id)) NameByID[id] = value; }
        public void SetParent(uint id, uint value) { if (_gameObjects.Contains(id) && NameByID.ContainsKey(id))  ParentByID[id] = value; }


        public static Scene GetScene(uint id) => SceneByID.ContainsKey(id) ? SceneByID[id] : null;
        public static Scene GetScene(string name) => GetScene(IDByName.ContainsKey(name) ? IDByName[name] : 0);
    }
}
