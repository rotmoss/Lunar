using System;
using System.Collections.Generic;
using Lunar.IO;
using System.IO;

namespace Lunar.Scene
{
    public struct ScriptInfo
    {
        public uint id;
        public (string, string)[] vars;
        public string className;
    }
    public partial class SceneController
    {
        private static SceneController instance = null;
        public static SceneController Instance { get { instance ??= new SceneController(); return instance; } }

        public IdCollection Ids { get => _ids; internal set => _ids = value; }
        private IdCollection _ids;

        private Dictionary<uint, string> _names;
        private Dictionary<uint, bool> _enabled;
        private Dictionary<uint, uint> _parent;
        private Dictionary<uint, uint> _scene;

        private SceneController()
        {
            _ids = new IdCollection();

            _names = new Dictionary<uint, string>();
            _enabled = new Dictionary<uint, bool>();
            _parent = new Dictionary<uint, uint>();
            _scene = new Dictionary<uint, uint>();
        }

        public void LoadScene(string file, out ScriptInfo[] scripts)
        {
            uint scene = _ids.GetId();

            if (!_names.ContainsKey(scene)) _names.Add(scene, file);
            else { _names[scene] = file; }
            if (!_enabled.ContainsKey(scene)) _enabled.Add(scene, true);
            else { _enabled[scene] = true; }

            LoadEntites(file, scene, out scripts);
        }

        internal void LoadEntites(string file, uint scene, out ScriptInfo[] scripts)
        {
            if (!File.Exists(FileManager.Path + "Scenes" + FileManager.Seperator + file))
            { Console.WriteLine("Could not find scene " + file); scripts = null; return; }

            XmlElementEntity[] array = FileManager.Dezerialize<XmlElementScene>(file, "Scenes", "scene").entities;
            List<ScriptInfo> scriptList = new List<ScriptInfo>();

            foreach (XmlElementEntity entity in array)
            {
                uint id = _ids.GetId();

                if (!_names.ContainsKey(id)) _names.Add(id, entity.Name);
                else { _names[id] = entity.Name; }

                if (!_enabled.ContainsKey(id)) _enabled.Add(id, entity.Enabled);
                else { _enabled[id] = entity.Enabled; }

                if (!_scene.ContainsKey(id)) _scene.Add(id, scene);
                else { _scene[id] = scene; }

                Transform.AddTransform(id);

                List<(string, string)> variables = new List<(string, string)>();
                if (entity.Script.Vars != null)
                    foreach (XmlElementVar var in entity.Script.Vars) variables.Add((var.Name, var.Value));
                scriptList.Add(new ScriptInfo { id = id, className = entity.Script.File, vars = variables.ToArray() });
            }

            scripts = scriptList.ToArray();

            foreach (XmlElementEntity entity in array)
            {
                if (string.IsNullOrEmpty(entity.Parent)) continue;

                uint id = GetEntityID(entity.Name);
                uint parentId = GetEntityID(entity.Parent);

                if (!_parent.ContainsKey(id)) _parent.Add(id, parentId);
                else { _parent[id] = parentId; }
            }
        }

        public void ForEach(Action<uint> action) => _ids.Values.ForEach(action);
    }
}
