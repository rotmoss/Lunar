using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Lunar.Xml;

namespace Lunar.ECS
{
    public class Scene : ICollectionItem
    {
        public static SceneCollection Collection = new SceneCollection();

        public event Action Disposed;

        public ICollectionItem Ancestor { get => _ancestor; set => _ancestor = value; }
        private ICollectionItem _ancestor;

        public ICollectionItem Parent { get => _parent; set => _parent = value; }
        private ICollectionItem _parent;

        public Guid Id { get => _id; set => _id = value; }
        private Guid _id;

        public bool Enabled { get => _enabled;  set => _enabled = value; }
        private bool _enabled;

        public string Name { get => _name; set => _name = value; }
        private string _name;

        public Scene(string name) 
        {
            _id = Guid.NewGuid();
            _enabled = true;
            _name = name;
            Disposed = null;
        }

        public void Dispose() 
        {
            Disposed?.Invoke();
            _id = Guid.Empty;
            _enabled = false;
            _name = "";
            Disposed = null;
        }
        
        public bool Equals(ICollectionItem obj) => obj.GetType() == typeof(Scene) && obj.Id == _id;

        public static void LoadScene(string fileName) 
        {
            XmlScene xmlScene;

            XmlSerializer serializer = new XmlSerializer(typeof(XmlScene), new XmlRootAttribute("scene"));
            using (StreamReader reader = new StreamReader(Engine.Path + "Scenes" + Engine.Seperator + fileName)) 
            {
                try {
                   xmlScene = (XmlScene)serializer.Deserialize(reader);
                }
                catch (InvalidOperationException e) {
                    System.Console.WriteLine(e.Message);
                    System.Console.WriteLine(e.InnerException.Message);
                    return;
                }
            }

            Scene scene = new Scene(fileName.Split('.')[0]);
            Scene.Collection.Add(scene);

            //Stores gameobject and name of parent
            List<(Gameobject, string)> gameobjects = new ();

            foreach(XmlGameobject xmlGameobject in xmlScene.Gameobjects) 
            {
                Gameobject gameobject = new Gameobject(xmlGameobject.Name);
                gameobject.Enabled = xmlGameobject.Enabled;

                foreach(XmlComponent xmlComponent in xmlGameobject.Components) 
                    xmlComponent.CreateComponent(gameobject);      

                gameobjects.Add(new (gameobject, xmlGameobject.Parent));
            }

            //Lookup gameobject from parent name and add to collection
            foreach((Gameobject, string) gameobject in gameobjects) 
            {
                ICollectionItem parent = gameobjects.Where(x => x.Item1.Name == gameobject.Item2).FirstOrDefault().Item1;
                if(parent == null) parent = scene;

                Gameobject.Collection.Add(gameobject.Item1, parent);
            }
        }
    }
}