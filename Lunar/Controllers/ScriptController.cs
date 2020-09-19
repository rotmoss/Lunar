using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lunar
{
    public partial class ScriptController
    {
        private static ScriptController instance = null;
        public static ScriptController Instance { get { instance ??= new ScriptController(); return instance; } }

        private Dictionary<uint, KeyValuePair<Script, bool>> _scripts;

        private Assembly _assembly;
        public Assembly Assembly { set { _assembly = value; } }

        private ScriptController()
        {
            _scripts = new Dictionary<uint, KeyValuePair<Script, bool>>();
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

            ((Script)script).Id = id;

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
                            case "System.Byte":
                                if (byte.TryParse(property.Item2, out byte byteResult)) {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { byteResult });
                                }
                                break;
                            case "System.Float":
                                if (float.TryParse(property.Item2, out float floatResult)) {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { floatResult });
                                }
                                break;
                            default:
                                try { type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { property.Item2 }); }
                                catch { continue; }
                                break;
                        }
                    }
                }
            }

            if (!_scripts.ContainsKey(id)) _scripts.Add(id, new KeyValuePair<Script, bool>((Script)script, true));
            else { _scripts[id] = new KeyValuePair<Script, bool>((Script)script, true); }
        }
        internal List<uint> GetRenderQueue() => _scripts.Values.Where(x => x.Key.Visible).Select(x => x.Key.Id).ToList();
        internal void Init() =>  _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.Init());
        internal void Update() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.Update());
        internal void LateUpdate() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.LateUpdate());
        internal void PostRender() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.PostRender());

        public void DisableScripts(uint callerId)
        {
            foreach (uint id in _scripts.Keys) if(id != callerId) _scripts[id] = new KeyValuePair<Script, bool>(_scripts[id].Key, false);
        }
        public void EnableScripts()
        {
            foreach (uint id in _scripts.Keys) _scripts[id] = new KeyValuePair<Script, bool>(_scripts[id].Key, true);
        }
    }
}
