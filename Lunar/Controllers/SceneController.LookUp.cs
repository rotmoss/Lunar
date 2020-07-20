namespace Lunar
{
    partial class SceneController
    {
        public uint GetEntityID(string name)
        {
            foreach (uint id in _name.Keys)
                if (_name[id] == name) return id;
            return 0;
        }
    }
}
