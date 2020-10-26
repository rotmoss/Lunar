using Lunar.Scenes;
using Lunar.Math;
using System.Linq;
using Lunar.Input;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Numerics;
using System.ComponentModel;

namespace Lunar.Scripts
{
    public abstract class Script
    {
        public uint Id { get; internal set; }
        public Scene Scene { get => Scene.GetScene(Id); }
        public uint Parent { get => Scene.GetParent(Id); set => Scene.SetParent(Id, value); }
        public string Name { get => Scene.GetName(Id); set => Scene.SetName(Id, value); }
        public static Assembly Assembly { get;  set; }
        private static Dictionary<uint, KeyValuePair<Script, bool>> _scripts = new Dictionary<uint, KeyValuePair<Script, bool>>();

        virtual public void Init() { }
        virtual public void LateInit() { }
        virtual public void Update() { }
        virtual public void LateUpdate() { }
        virtual public void PostRender() { }

        public static void InitScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.Init());
        public static void LateInitScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.LateInit());
        public static void UpdateScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.Update());
        public static void LateUpdateScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.LateUpdate());
        public static void PostRenderUpdateScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.PostRender());

        public static void AddScripts(ScriptInfo[] scripts)
        {
            foreach (ScriptInfo script in scripts) {
                AddScript(script.id, script.className, script.vars);
            }
        }

        internal static void AddScript(uint id, string className, (string, string)[] vars = null)
        {
            object script;

            if (Assembly == null) return;
            try { script = Assembly.CreateInstance("Lunar." + className); }
            catch { return; }

            if (!script.GetType().IsSubclassOf(typeof(Script)))
            { Console.WriteLine("Script " + className + " does not inherit from script"); return; }

            ((Script)script).Id = id;

            if (vars != null)
            {
                Type type = script.GetType();
                PropertyInfo[] members = type.GetProperties();
                foreach ((string, string) variable in vars)
                {
                    PropertyInfo member = members.Where(x => x.Name == variable.Item1).FirstOrDefault();

                    if (member != default)
                        try { type.InvokeMember(variable.Item1, BindingFlags.SetField, null, script, new object[] { Convert.ChangeType(variable.Item2, member.PropertyType) }); }
                        catch { Console.WriteLine("Wrong type for script variable: " + variable.Item1); }
                }
            }

            if (!_scripts.ContainsKey(id)) _scripts.Add(id, new KeyValuePair<Script, bool>((Script)script, true));
            else { _scripts[id] = new KeyValuePair<Script, bool>((Script)script, true); }
        }

        public static uint GetId(Script script) => _scripts.Where(x => x.Value.Equals(script)).Select(x => x.Key).FirstOrDefault();
        public static Script GetScript(uint id) => _scripts[id].Key;
        public static T[] GetScriptsByType<T>() where T : Script => _scripts.Where(x => x.Value.Key.GetType() == typeof(T)).Select(x => (T)x.Value.Key).ToArray();

        public static void DisableScripts(uint callerId)
        {
            foreach (uint id in _scripts.Keys) if (id != callerId) _scripts[id] = new KeyValuePair<Script, bool>(_scripts[id].Key, false);
        }
        public static void EnableScripts()
        {
            foreach (uint id in _scripts.Keys) _scripts[id] = new KeyValuePair<Script, bool>(_scripts[id].Key, true);
        }

        public bool GetKeyState(Key key) => InputController.GetKeyState(key);
        public bool GetKeyState(SDL2.SDL.SDL_Keycode key) => InputController.GetKeyState(key);
        public bool GetButtonState(Button button, int id) => InputController.GetButtonState(button, id);
        public float GetAxisState(Axis axsis, int id) => InputController.GetAxisState(axsis, id);

        public Transform Transform { get => Scene.GetGlobalTransform(Id); set => Scene.SetTransform(Id, value); }
        public Transform LocalTransform { get => Scene.GetLocalTransform(Id); set => Scene.SetTransform(Id, value); }
    }
}
