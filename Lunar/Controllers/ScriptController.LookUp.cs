using System.Linq;

namespace Lunar
{
    partial class ScriptController
    {
        public uint GetId(Script script) =>_scripts.Where(x => x.Value.Equals(script)).Select(x => x.Key).FirstOrDefault();
        public Script GetScript(uint id) => _scripts[id].Key;
        public T[] GetScriptsByType<T>() where T : Script => _scripts.Where(x => x.Value.Key.GetType() == typeof(T)).Select(x => (T)x.Value.Key).ToArray();
    }
}
