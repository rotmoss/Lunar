using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunar.IO;

namespace Lunar.ECS
{
    public class Scene : ObjectIdentifier<Scene>
    {
        public string Name { get => _name; set => _name = value; }
        private string _name;

        public Scene(string file) : base()
        {
            XmlScene scene = FileManager.Deserialize(file, "Scenes");

            Name = file;
            Enabled = true;
        }

        public void DisposeChild() { }

        public void LoadScene(string file)
        {
            XmlScene scene = FileManager.Deserialize(file, "Scenes");

            foreach (XmlGameObject xmlGameobject in scene.Gameobjects)
                Gameobject.Collection.AddEntry(this, new Gameobject(xmlGameobject));

            foreach (XmlGameObject gameobject in scene.Gameobjects)
                Gameobject.HierarchyTree.SetParent(Gameobject.GetId(gameobject.Name), Gameobject.GetId(gameobject.Parent));
        }

        public override void DerivedDispose() { }
    }
}
