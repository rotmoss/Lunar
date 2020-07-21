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

        private Dictionary<uint, Script> _scripts;

        private Assembly _assembly;
        public Assembly Assembly { set { _assembly = value; } }

        private ScriptController()
        {
            _scripts = new Dictionary<uint, Script>();
        }

        internal void AddScript(uint id, string className, (string, string)[] properties = null)
        {
            object script;

            if (_assembly == null) return;
            try { script = _assembly.CreateInstance("Lunar." + className); }
            catch { return; }

            if (script == null) return;
            if (!script.GetType().IsSubclassOf(typeof(Script)))
            { Console.WriteLine("Script " + className + " does not inherit from script"); return; }

            ((Script)script)._id = id;

            if (properties != null)
            {
                Type type = script.GetType();
                foreach ((string, string) property in properties)
                {
                    if (type.GetMembers().Select(x => x.Name).Contains(property.Item1))
                    {
                        switch (type.GetMember(property.Item1)[0].GetUnderlyingType().ToString())
                        {
                            case "System.Bool":
                                if (bool.TryParse(property.Item2, out bool boolResult)) {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { boolResult });
                                }
                                break;
                            case "System.Int32":
                                if (int.TryParse(property.Item2, out int intResult)) {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { intResult });
                                }
                                break;
                            case "System.Float":
                                if (float.TryParse(property.Item2, out float floatResult)) {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { floatResult });
                                }
                                break;
                            case "System.String":
                                type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { property.Item2 });
                                break;
                        }
                    }
                }
            }

            if (!_scripts.ContainsKey(id)) _scripts.Add(id, (Script)script);
            else { _scripts[id] = (Script)script; }
        }
        internal List<uint> GetRenderQueue() => _scripts.Values.Where(x => x._visible).Select(x => x._id).ToList();
        internal void UpdateDeltaTime(float deltaTime) => _scripts.Values.ToList().ForEach(x => x.DeltaTime = deltaTime);

        internal void Init() =>  _scripts.Values.ToList().ForEach(x => x.Init());
        internal void Update() => _scripts.Values.ToList().ForEach(x => x.Update());
        internal void LateUpdate() => _scripts.Values.ToList().ForEach(x => x.LateUpdate());
        internal void PostRender() => _scripts.Values.ToList().ForEach(x => x.PostRender());
    }
}
