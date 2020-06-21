using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar
{
    public partial class SceneController
    {
        private static SceneController instance = null;
        public static SceneController Instance { get { instance = instance == null ? new SceneController() : instance; return instance; } }

        private IdCollection _enteties;

        private Dictionary<uint, string> _name;
        private Dictionary<uint, bool> _enabled;
        private Dictionary<uint, uint> _parent;
        private Dictionary<uint, uint> _scene;

        public SceneController()
        {
            _enteties = new IdCollection();

            _name = new Dictionary<uint, string>();
            _enabled = new Dictionary<uint, bool>();
            _parent = new Dictionary<uint, uint>();
            _scene = new Dictionary<uint, uint>();
        }

        public void LoadScene(string file)
        {
            if (!FileManager.ReadLines(file, "Scenes", out string[] result))
            { Console.WriteLine("Could not find scene " + file); return; }

            uint id = _enteties.GetId();

            if (!_name.ContainsKey(id)) _name.Add(id, "");
            _name[id] = file;

            if (!_enabled.ContainsKey(id)) _enabled.Add(id, true);

            LoadEntites(result, id);
        }

        public void LoadEntites(string[] file, uint scene)
        {
            List<uint> ids = new List<uint>();

            foreach (string s in file)
            {
                string[] tokens = s.Split(' ');
                if (tokens[0] != "entity") continue;

                uint id = _enteties.GetId();

                foreach (string token in tokens)
                {
                    string[] values = token.Split(new char[] { '=', '"' }).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    if (values[0] == "name") { 
                        if (!_name.ContainsKey(id)) _name.Add(id, ""); 
                        _name[id] = values[1]; 
                    }
                    if (values[0] == "enabled") { 
                        if (!_enabled.ContainsKey(id)) _enabled.Add(id, true); 
                        bool.TryParse(values[1], out bool enabled);
                        _enabled[id] = enabled;
                    }
                    if (values[0] == "parent") { 
                        if (!_parent.ContainsKey(id)) _parent.Add(id, 0); 
                        _parent[id] = GetEntityID(values[1]); 
                    }
                    if (values[0] == "script") {
                        ScriptController.Instance.AddScript(id, values[1]);
                    }
                }
                if (!_scene.ContainsKey(id)) _scene.Add(id, 0);
                _scene[id] = scene;
            }
        }

        public void ForEach(Action<uint> action) => _enteties.Values.ForEach(action);
    }
}
