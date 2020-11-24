using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Lunar.Scripts
{
    public class Script : Component<Script>
    {
        public static Assembly Assembly { get;  set; }
        private ScriptTemplate _script;

        public Script(string className, Dictionary<string, string> vars = null) : base()
        {
            if (Assembly == null) return;

            object scriptObject = Assembly.CreateInstance("Lunar." + className);

            if (scriptObject == null) { 
                Console.WriteLine(className + ".cs could not be found");
                return; 
            }

            if (!scriptObject.GetType().IsSubclassOf(typeof(ScriptTemplate))) { 
                Console.WriteLine(className + " does not inherit from ScriptTemplate"); 
                return; 
            }

            _script = (ScriptTemplate)scriptObject;
            _script._container = this;

            if (vars != null) SetVars(vars);
        }

        public void SetVars(Dictionary<string, string> vars)
        {
            PropertyInfo[] members = _script.GetType().GetProperties();
            foreach (string key in vars.Keys)
            {
                PropertyInfo member = members.Where(x => x.Name == key).FirstOrDefault();

                if (member != default)
                    try { _script.GetType().InvokeMember(key, BindingFlags.SetField, null, _script, new object[] { Convert.ChangeType(vars[key], member.PropertyType) }); }
                    catch { Console.WriteLine("Wrong type for script variable: " + key); }            
            }
        }

        public override void DisposeChild() { }

        public static void DisableScripts(uint callerId) => _components.ForEach(x => x.Enabled = x.Id != callerId ? false : true);
        public static void EnableScripts() => _components.ForEach(x => x.Enabled = true);

        public void Init() => _script.Init();
        public void LateInit() => _script.LateInit();

        public void Update() => _script.Update();
        public void LateUpdate() => _script.LateUpdate();
        public void PostRender() => _script.PostRender();

        public static void InitScripts() { foreach(Script component in _components) component.Init(); }
        public static void LateInitScripts() { foreach (Script component in _components) component.LateInit(); }

        public static void UpdateScripts() { foreach (Script component in _components.Where(x => x.Enabled)) component.Update(); }
        public static void LateUpdateScripts() { foreach(Script component in _components.Where(x => x.Enabled)) component.LateUpdate(); }
        public static void PostRenderUpdateScripts() { foreach (Script component in _components.Where(x => x.Enabled)) component.PostRender(); }
    }
}
