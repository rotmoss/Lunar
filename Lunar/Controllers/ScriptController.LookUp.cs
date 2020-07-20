using System.Collections.Generic;
using System.Linq;

namespace Lunar
{
    partial class ScriptController
    {
        public uint GetId(Script script) =>_scripts.Where(x => x.Value.Equals(script)).Select(x => x.Key).FirstOrDefault();
        public Script[] GetScriptsByType<T>() => _scripts.Where(x => x.Value.GetType().Equals(typeof(T))).Select(x => x.Value).ToArray();
    }
}
