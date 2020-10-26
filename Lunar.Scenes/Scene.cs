using System;
using System.Collections.Generic;
using Lunar.IO;
using System.IO;
using System.Linq;
using Lunar.Math;
using OpenGL;

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

        private static Dictionary<uint, Scene> SceneByID = new Dictionary<uint, Scene>();
        private static Dictionary<uint, uint> ParentByID = new Dictionary<uint, uint>();
        private static Dictionary<uint, string> NameByID = new Dictionary<uint, string>();
        private static Dictionary<string, uint> IDByName = new Dictionary<string, uint>();
        private static Dictionary<uint, Transform> TransformByID  = new Dictionary<uint, Transform>();
        private static IdCollection IDs = new IdCollection();
        private static List<Scene> Scenes = new List<Scene>();

        public Scene()
        {
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

                TransformByID.Add(gameObject, new Transform());

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
        public uint[] GetParents(uint id) { List<uint> parents = new List<uint>(); while (id < 0) { id = GetParent(id); parents.Add(id); } return parents.ToArray(); }
        public uint GetId(string name) =>  IDByName.ContainsKey(name.ToLower()) ? _gameObjects.Contains(IDByName[name.ToLower()]) ? IDByName[name.ToLower()] : 0: 0;
        public uint GetParent(string name) => GetParent(GetId(name));
        public uint[] GetParents(string name) { List<uint> parents = new List<uint>(); id = GetParent(GetId(name)); while (id < 0) { parents.Add(id); id = GetParent(id); } return parents.ToArray(); }

        public void SetName(uint id, string value) { if (_gameObjects.Contains(id) && NameByID.ContainsKey(id)) NameByID[id] = value; }
        public void SetParent(uint id, uint value) { if (_gameObjects.Contains(id) && NameByID.ContainsKey(id))  ParentByID[id] = value; }
        public static Transform GetLocalTransform(uint id) => TransformByID.ContainsKey(id) ? TransformByID[id] : Transform.Zero;
        public static Transform GetGlobalTransform(uint id)
        {
            Scene scene = GetScene(id);

            if (scene == null)
                return Transform.Zero;

            Transform t = Transform.Zero;
            foreach (uint parent in scene.GetParents(id))
                t += GetLocalTransform(id);
            
            return t;
        }
        public static void SetTransform(uint id, Transform value) { if (TransformByID.ContainsKey(id)) TransformByID[id] = value; }
        public static void MoveTransform(uint id, Vertex2f value) { if (TransformByID.ContainsKey(id)) TransformByID[id] += value; }
        public static void ScaleTransform(uint id, Vertex2f value) { if (TransformByID.ContainsKey(id)) TransformByID[id] *= value; }
        public static void ScaleTransform(uint id, float value) { if (TransformByID.ContainsKey(id)) TransformByID[id] *= value; }
        public static Scene GetScene(uint id) => SceneByID.ContainsKey(id) ? SceneByID[id] : null;
        public static Scene GetScene(string name) => GetScene(IDByName.ContainsKey(name) ? IDByName[name] : 0);
    }
}
