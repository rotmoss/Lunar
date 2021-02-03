using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Lunar.ECS.Components
{
    public class Script : Component<Script>
    {
        public static Assembly Assembly { get;  set; }
        private ScriptTemplate _script;

        public Script(string className, Dictionary<string, string> variables) : base()
        {
            if (Assembly == null) return;

            _script = LoadScript(className, variables);
            SetVars(_script, variables);
            _script.Container = this;
        }

        public ScriptTemplate LoadScript(string className, Dictionary<string, string> variables)
        {
            object scriptObject = Assembly.CreateInstance("Lunar." + className);

            if (scriptObject == null) {
                Console.WriteLine(className + ".cs could not be found");
                return default;
            }

            if (!scriptObject.GetType().IsSubclassOf(typeof(ScriptTemplate))) {
                Console.WriteLine(className + " does not inherit from ScriptTemplate");
                return default;
            }

            return (ScriptTemplate)scriptObject;
        }

        public void SetVars(ScriptTemplate script, Dictionary<string, string> vars)
        {
            if (vars == null || script == null) return;

            PropertyInfo[] members = script.GetType().GetProperties();
            foreach (string key in vars.Keys)
            {
                PropertyInfo member = members.Where(x => x.Name == key).FirstOrDefault();

                if (member != default)
                    try { script.GetType().InvokeMember(key, BindingFlags.SetField, null, script, new object[] { Convert.ChangeType(vars[key], member.PropertyType) }); }
                    catch { Console.WriteLine("Wrong type for script variable: " + key); }            
            }
        }

        public override void DisposeChild() 
        { 
            _script = null;
        }

        public static void DisableScripts(uint callerId) {
            foreach(Script scriptConainer in _entries)
                scriptConainer.Enabled = scriptConainer.Id == callerId;
        }
        public static void EnableScripts(uint callerId)
        {
            foreach (Script scriptContainer in _entries)
                scriptContainer.Enabled = true;
        }

        public static void InitScripts() { foreach(Script scriptContainter in _entries) scriptContainter._script.Init(); }
        public static void LateInitScripts() { foreach(Script scriptContainter in _entries) scriptContainter._script.LateInit(); }

        public static void UpdateScripts() { foreach (Script scriptContainter in _entries.Where(x => x.Enabled)) scriptContainter._script.Update(); }
        public static void LateUpdateScripts() { foreach (Script scriptContainter in _entries.Where(x => x.Enabled)) scriptContainter._script.LateUpdate(); }
        public static void PostRenderUpdateScripts() { foreach (Script scriptContainter in _entries.Where(x => x.Enabled)) scriptContainter._script.PostRender(); }
    }
}
