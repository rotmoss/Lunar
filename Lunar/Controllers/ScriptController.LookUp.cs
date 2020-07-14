using System.Collections.Generic;

namespace Lunar
{
    partial class ScriptController
    {
        public uint GetId(Script script)
        {
            foreach (KeyValuePair<uint, Script[]> pair in _scripts)
            {
                foreach (Script value in pair.Value)
                    if (script.Equals(value)) return pair.Key;
            }
            return 0;
        }

        public Script[] GetScriptsByType<T>()
        {
            List<Script> scripts = new List<Script>();

            foreach (KeyValuePair<uint, Script[]> pair in _scripts)
                foreach (Script script in pair.Value)
                    if (script.GetType() == typeof(T)) scripts.Add(script);

            return scripts.ToArray();
        }
    }
}
