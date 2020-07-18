using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lunar
{
    public partial class ScriptController
    {
        private static ScriptController instance = null;
        public static ScriptController Instance { get { instance = instance == null ? new ScriptController() : instance; return instance; } }

        private Dictionary<uint, Script[]> _scripts;

        private Assembly _assembly;
        public Assembly Assembly { set { _assembly = value; } }

        private ScriptController()
        {
            _scripts = new Dictionary<uint, Script[]>();
        }

        internal void AddScript(uint id, string className)
        {
            object script;

            if (_assembly == null) return;
            try { script = _assembly.CreateInstance("Lunar." + className); }
            catch { return; }

            if (script == null) return;
            if (!script.GetType().IsSubclassOf(typeof(Script)))
            { Console.WriteLine("Script " + className + " does not inherit from script"); return; }

            ((Script)script)._id = id;
   
            if (!_scripts.ContainsKey(id)) _scripts.Add(id, new Script[0]);
            _scripts[id] = _scripts[id].Add((Script)script);
        }

        internal Dictionary<uint, Transform> GetTransforms()
        {
            Dictionary<uint, Transform> temp = new Dictionary<uint, Transform>();

            foreach (KeyValuePair<uint, Script[]> pair in _scripts) {
                foreach (Script script in pair.Value)
                    temp.Add(pair.Key, script._transform);
            }

            return temp;
        }

        internal List<uint> GetRenderQueue()
        {
            List<uint> result = new List<uint>();

            foreach(Script[] scripts in _scripts.Values)
                result.AddRange(scripts.Where(x => x._visible).Select(x => x._id));
            
            return result;
        }

        internal void UpdateDeltaTime(float deltaTime)
        {
            foreach (Script[] scripts in _scripts.Values)
                scripts.ToList().ForEach(x => x.DeltaTime = deltaTime);
        }

        internal void Init() =>  _scripts.Values.ToList().ForEach(x => x.ToList().ForEach(x => x.Init()));
        internal void Update() => _scripts.Values.ToList().ForEach(x => x.ToList().ForEach(x => x.Update()));
        internal void LateUpdate() => _scripts.Values.ToList().ForEach(x => x.ToList().ForEach(x => x.LateUpdate()));
        internal void PostRender() => _scripts.Values.ToList().ForEach(x => x.ToList().ForEach(x => x.PostRender()));
    }
}
