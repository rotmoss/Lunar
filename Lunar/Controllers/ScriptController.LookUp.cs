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

        public Transform GetTransform(uint id)
        {
            if (_transform.ContainsKey(id)) return _transform[id];
            return null;
        }

        public Transform GetTransform(Script script)
        {
            if (_transform.ContainsKey(script._id)) return _transform[script._id];
            return null;
        }
    }
}
