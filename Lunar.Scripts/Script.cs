using Lunar.Scenes;
using System.Linq;
using Lunar.Input;
using System.Collections.Generic;
using System.Reflection;
using System;
using Lunar.Transforms;

namespace Lunar.Scripts
{
    public class Script
    {
        public uint Id { get; internal set; }
        public bool Enabled { get; set; }
        public Scene Scene { get => Scene.GetScene(Id); }
        public uint Parent { get => Scene.GetParent(Id); set => Scene.SetParent(Id, value); }
        public string Name { get => Scene.GetName(Id); set => Scene.SetName(Id, value); }
        public static Assembly Assembly { get;  set; }
        private static Dictionary<uint, List<Script>> _scripts = new Dictionary<uint, List<Script>>();

        public static void CreateScript(uint id, string className, (string, string)[] vars = null)
        {
            object scriptObject;

            if (Assembly == null) return;

            scriptObject = Assembly.CreateInstance("Lunar." + className);

            if (scriptObject == null)
            { Console.WriteLine("Script " + className + ".cs could not be found"); return; }

            if (!scriptObject.GetType().IsSubclassOf(typeof(Script)))
            { Console.WriteLine("Script " + className + " does not inherit from script"); return; }

            Script script = (Script)scriptObject;

            script.Id = id;
            Transform.AddTransform(id, new Transform());

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

            if (!_scripts.ContainsKey(id)) { _scripts.Add(id, new List<Script>()); }
            _scripts[id].Add(script);
        }

        virtual public void Init() { }
        virtual public void LateInit() { }
        virtual public void Update() { }
        virtual public void LateUpdate() { }
        virtual public void PostRender() { }

        public static void InitScripts() { foreach (List<Script> scripts in _scripts.Values) foreach(Script script in scripts) script.Init(); }
        public static void LateInitScripts() { foreach (List<Script> scripts in _scripts.Values) foreach (Script script in scripts) script.LateInit(); }
        public static void UpdateScripts() { foreach (List<Script> scripts in _scripts.Values) foreach (Script script in scripts.Where(x => x.Enabled)) script.Update(); }
        public static void LateUpdateScripts() { foreach (List<Script> scripts in _scripts.Values) foreach(Script script in scripts.Where(x => x.Enabled)) script.LateUpdate(); }
        public static void PostRenderUpdateScripts() { foreach (List<Script> scripts in _scripts.Values) foreach (Script script in scripts.Where(x => x.Enabled)) script.PostRender(); }
        public static void AddScripts(List<ScriptInfo> scripts) { if (scripts != null) scripts.ForEach(x => CreateScript(x.id, x.className, x.vars)); }
        public static uint GetId(Script script) => _scripts.Where(x => x.Value.Equals(script)).Select(x => x.Key).FirstOrDefault();
        public static Script[] GetScripts(uint id) => _scripts[id].ToArray();
        public static T[] GetScriptsByType<T>() where T : Script => (T[])_scripts.Values.ToList().SelectMany(x => x).Where(x => x.GetType() == typeof(T)).ToArray();
        public static void DisableScripts(uint callerId) => _scripts.Values.ToList().ForEach(x => x.ForEach(x => x.Enabled = x.Id != callerId ? false : true));
        public static void EnableScripts() => _scripts.Values.ToList().ForEach(x => x.ForEach(x => x.Enabled = true));
        public bool GetKeyState(Key key) => InputController.GetKeyState(key);
        public bool GetKeyState(SDL2.SDL.SDL_Keycode key) => InputController.GetKeyState(key);
        public bool GetButtonState(Button button, int id) => InputController.GetButtonState(button, id);
        public float GetAxisState(Axis axsis, int id) => InputController.GetAxisState(axsis, id);

        public Transform Transform { get => Transform.GetGlobalTransform(Id); set => Transform.SetTransform(Id, value); }
        public Transform LocalTransform { get => Transform.GetLocalTransform(Id); set => Transform.SetTransform(Id, value); }
    }
}
