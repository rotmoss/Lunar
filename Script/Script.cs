using Lunar.Scene;
using System.Linq;
using Lunar.Input;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Numerics;

namespace Lunar.Scripts
{
    public abstract class Script
    {
        public uint Id { get; internal set; }
        public string Name { get; internal set; }
        public static Assembly Assembly { get;  set; }
        private static Dictionary<uint, KeyValuePair<Script, bool>> _scripts = new Dictionary<uint, KeyValuePair<Script, bool>>();

        virtual public void Init() { }
        virtual public void Update() { }
        virtual public void LateUpdate() { }
        virtual public void PostRender() { }

        public static void InitScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.Init());
        public static void UpdateScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.Update());
        public static void LateUpdateScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.LateUpdate());
        public static void PostRenderUpdateScripts() => _scripts.Values.Where(x => x.Value).ToList().ForEach(x => x.Key.PostRender());

        public static void AddScripts(ScriptInfo[] scripts)
        {
            foreach (ScriptInfo script in scripts) {
                AddScript(script.id, script.className, script.vars);
            }
        }

        internal static void AddScript(uint id, string className, (string, string)[] properties = null)
        {
            object script;

            if (Assembly == null) return;
            try { script = Assembly.CreateInstance("Lunar." + className); }
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
                        switch (GetUnderlyingType(type.GetMember(property.Item1)[0]).ToString())
                        {
                            case "System.Bool":
                                if (bool.TryParse(property.Item2, out bool boolResult))
                                {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { boolResult });
                                }
                                break;
                            case "System.Int32":
                                if (int.TryParse(property.Item2, out int intResult))
                                {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { intResult });
                                }
                                break;
                            case "System.Byte":
                                if (byte.TryParse(property.Item2, out byte byteResult))
                                {
                                    type.InvokeMember(property.Item1, BindingFlags.SetField, null, script, new object[] { byteResult });
                                }
                                break;
                            case "System.Float":
                                if (float.TryParse(property.Item2, out float floatResult))
                                {
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

        public static Type GetUnderlyingType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
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

        public Transform Transform { get => Transform.GetGlobalTransform(Id); set => Transform.SetTransform(Id, value); }
        public Transform LocalTransform { get => Transform.GetLocalTransform(Id); set => Transform.SetTransform(Id, value); }
        public uint Parent { get => SceneController.Instance.GetEntityParent(Id); set => SceneController.Instance.SetEntityParent(Id, value); }
        public uint GetEntityId(string name) => SceneController.Instance.GetEntityID(name);
        public string GetEntityName(uint id) => SceneController.Instance.GetEntityName(id);

    }
}
