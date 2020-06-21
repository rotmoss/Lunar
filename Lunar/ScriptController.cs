using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public abstract class Script
    {
        public uint _id;
        public bool _render;

        public Transform _transform;

        abstract public void Init();
        abstract public void Update();
        abstract public void LateUpdate();


        public void CreateSprite(string texture, string vShader, string fShader) => Graphics.Instance.CreateSprite(_id, texture, vShader, fShader);
        public void ScaleSprite(float w, float h) => Graphics.Instance.ScaleMatrix(_id, w, h);
    }

    class ScriptController
    {
        private static ScriptController instance = null;
        public static ScriptController Instance { get { instance = instance == null ? new ScriptController() : instance; return instance; } }

        private Dictionary<uint, Script[]> _scripts;
        private Dictionary<uint, Transform> _transform;

        private Assembly _assembly;
        public Assembly Assembly { set { _assembly = value; } }

        public ScriptController()
        {
            _scripts = new Dictionary<uint, Script[]>();
            _transform = new Dictionary<uint, Transform>();
        }

        public void AddScript(uint id, string className)
        {
            object script;

            if (_assembly == null) return;
            try { script = _assembly.CreateInstance("Lunar." + className); }
            catch { return; }

            if (script == null) return;
            if (!script.GetType().IsSubclassOf(typeof(Script)))
            { Console.WriteLine("Script " + className + " does not inherit from script"); return; }

            ((Script)script)._id = id;

            if (!_transform.ContainsKey(id)) _transform.Add(id, new Transform(0, 0, 0, 1, 1));
            ((Script)script)._transform = _transform[id];

            if (!_scripts.ContainsKey(id)) _scripts.Add(id, new Script[0]);
            _scripts[id] = _scripts[id].Add((Script)script);
        }

        public uint GetId(Script script)
        {
            foreach(KeyValuePair<uint, Script[]> pair in _scripts)
            {
                foreach(Script value in pair.Value)
                    if (script.Equals(value)) return pair.Key;
            }
            return 0;
        }

        public Transform GetTransform(uint id)
        {
            if (_transform.ContainsKey(id)) return _transform[id];
            return null;
        }

        public void UpdateTransforms(Dictionary<uint, Transform> transforms)
        {
            foreach (KeyValuePair<uint, Transform> pair in transforms) {
                if (_scripts.ContainsKey(pair.Key)) {
                    foreach(Script script in _scripts[pair.Key])
                        script._transform = pair.Value;
                }
            }
        }

        public Dictionary<uint, Transform> GetTransforms()
        {
            Dictionary<uint, Transform> temp = new Dictionary<uint, Transform>();

            foreach (KeyValuePair<uint, Script[]> pair in _scripts) {
                foreach (Script script in pair.Value)
                    temp.Add(pair.Key, script._transform);
            }

            return temp;
        }

        public List<uint> GetRenderQueue()
        {
            List<uint> result = new List<uint>();
            _scripts.Values.ToList().ForEach(x => result.AddRange(x.Where(x => x._render).Select(x => x._id).ToArray()));
            return result;
        }

        public void InitScripts() =>  _scripts.Values.ToList().ForEach(x => x.ToList().ForEach(x => x.Init()));
        public void UpdateScripts() => _scripts.Values.ToList().ForEach(x => x.ToList().ForEach(x => x.Update()));
        public void LateUpdateScripts() => _scripts.Values.ToList().ForEach(x => x.ToList().ForEach(x => x.LateUpdate()));
    }
}
