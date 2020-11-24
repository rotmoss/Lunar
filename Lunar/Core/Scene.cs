using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunar.IO;

namespace Lunar
{
    public class Scene : IDisposable
    {
        public uint Id { get; }

        private static IdCollection _idCollection = new IdCollection();

        private static Dictionary<uint, string> _name = new Dictionary<uint, string>();
        private static Dictionary<uint, bool> _enabled = new Dictionary<uint, bool>();
        private static Dictionary<uint, XmlScene> _xmlScene = new Dictionary<uint, XmlScene>();
        private static Dictionary<uint, List<Gameobject>> _gameobjects = new Dictionary<uint, List<Gameobject>>();

        public Scene(string file)
        {
            Id = _idCollection.GetId();

            _enabled.Add(Id, false);
            _name.Add(Id, file);
            _xmlScene.Add(Id, FileManager.Deserialize(file, "Scenes"));
            _gameobjects.Add(Id, new List<Gameobject>());

            foreach (XmlGameObject gameobject in _xmlScene[Id].Gameobjects)
                _gameobjects[Id].Add(new Gameobject(gameobject, Id));

            foreach (XmlGameObject gameobject in _xmlScene[Id].Gameobjects)
                Gameobject.SetParent(Gameobject.GetId(gameobject.Name), Gameobject.GetId(gameobject.Parent));
        }

        public void Dispose()
        {
            foreach (Gameobject gameobject in _gameobjects[Id])
                gameobject.Dispose();

            _gameobjects.Remove(Id);
            _xmlScene.Remove(Id);
        }

        public static uint GetId(string name) => _name.FirstOrDefault(x => x.Value == name).Key;
        public static string GetName(uint Id) => _name.ContainsKey(Id) ? _name[Id] : null;

        public static bool isEnabled(uint Id) => _enabled.ContainsKey(Id) ? _enabled[Id] : false;
        public static bool isEnabled(string name) => isEnabled(GetId(name));

        public static void Enable(uint Id) { if(_enabled.ContainsKey(Id)) _enabled[Id] = true; }
        public static void Disable(uint Id) { if (_enabled.ContainsKey(Id)) _enabled[Id] = false; }

        public static XmlScene GetXmlScene(uint Id) => _xmlScene.ContainsKey(Id) ? _xmlScene[Id] : null;
        public static XmlScene GetXmlScene(string name) => GetXmlScene(GetId(name));

        public static List<Gameobject> GetGameobjects(uint Id) => _gameobjects.ContainsKey(Id) ? _gameobjects[Id] : null;
        public static List<Gameobject> GetGameobjects(string name) => GetGameobjects(GetId(name));
    }
}
