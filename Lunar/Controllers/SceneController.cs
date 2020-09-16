using System;
using System.Collections.Generic;
using System.IO;

namespace Lunar
{
    public partial class SceneController
    {
        private static SceneController instance = null;
        public static SceneController Instance { get { instance ??= new SceneController(); return instance; } }

        private IdCollection _ids;

        private Dictionary<uint, string> _names;
        private Dictionary<uint, bool> _enabled;
        private Dictionary<uint, uint> _parent;
        private Dictionary<uint, uint> _scene;

        public Dictionary<uint, Transform> GlobalTransforms { 
            get 
            {
                Dictionary<uint, Transform> dictionary = new Dictionary<uint, Transform>();

                foreach (KeyValuePair<uint, Transform> pair in _transforms)
                    dictionary.Add(pair.Key, GetEntityGlobalTransform(pair.Key));

                return dictionary;
             } 
        }
        public Dictionary<uint, Transform> LocalTransforms { get => _transforms; }
        private Dictionary<uint, Transform> _transforms;

        public Dictionary<uint, bool> Visible { get => _visible; }
        private Dictionary<uint, bool> _visible;

        private SceneController()
        {
            _ids = new IdCollection();

            _names = new Dictionary<uint, string>();
            _enabled = new Dictionary<uint, bool>();
            _parent = new Dictionary<uint, uint>();
            _scene = new Dictionary<uint, uint>();

            _transforms = new Dictionary<uint, Transform>();
            _visible = new Dictionary<uint, bool>();
        }

        internal void LoadScene(string file)
        {
            uint scene = _ids.GetId();

            if (!_names.ContainsKey(scene)) _names.Add(scene, file);
            else { _names[scene] = file; }
            if (!_enabled.ContainsKey(scene)) _enabled.Add(scene, true);
            else { _enabled[scene] = true; }

            LoadEntites(file, scene);
        }

        internal void LoadEntites(string file, uint scene)
        {
            if (!File.Exists(FileManager.Path + "Scenes" + FileManager.Seperator + file))
            { Console.WriteLine("Could not find scene " + file); return; }

            XmlElementEntity[] array = FileManager.Dezerialize<XmlElementScene>(file, "Scenes", "scene").entities;

            foreach (XmlElementEntity entity in array)
            {
                uint id = _ids.GetId();

                if (!_names.ContainsKey(id)) _names.Add(id, entity.Name);
                else { _names[id] = entity.Name; }

                if (!_enabled.ContainsKey(id)) _enabled.Add(id, entity.Enabled);
                else { _enabled[id] = entity.Enabled; }

                if (!_scene.ContainsKey(id)) _scene.Add(id, scene);
                else { _scene[id] = scene; }

                if (!_transforms.ContainsKey(id)) _transforms.Add(id, new Transform(0, 0, 1, 1));
                else { _transforms[id] = new Transform(0, 0, 1, 1); }

                if (!_visible.ContainsKey(id)) _visible.Add(id, true);
                else { _visible[id] = true; }

                List<(string, string)> variables = new List<(string, string)>();
                if (entity.Script.Vars != null)
                    foreach (XmlElementVar var in entity.Script.Vars) variables.Add((var.Name, var.Value));
                ScriptController.Instance.AddScript(id, entity.Script.File, variables.ToArray());
            }

            foreach(XmlElementEntity entity in array)
            {
                if (string.IsNullOrEmpty(entity.Parent)) continue;

                uint id = GetEntityID(entity.Name);
                uint parentId = GetEntityID(entity.Parent);

                if (!_parent.ContainsKey(id)) _parent.Add(id, parentId);
                else { _parent[id] = parentId; }
            }
        }

        public Transform GetEntityLocalTransform(uint id) => _transforms.ContainsKey(id) ? _transforms[id] : Transform.Zero;
        public Transform GetEntityGlobalTransform(uint id) => _transforms.ContainsKey(id) ? _parent.ContainsKey(id) ? _transforms[id] + GetEntityLocalTransform(_parent[id]) : _transforms[id] : Transform.Zero;
        public void SetEntityTransform(uint id, Transform value) { if (_transforms.ContainsKey(id)) _transforms[id] = value; }
        public bool GetEntityVisibility(uint id) => _visible.ContainsKey(id) ? _visible[id] : false;
        public void SetEntityVisibility(uint id, bool value) { if (_visible.ContainsKey(id)) _visible[id] = value; }
        public void ForEach(Action<uint> action) => _ids.Values.ForEach(action);
    }
}
