using System.Xml.Serialization;
using Lunar.ECS;

namespace Lunar.Xml
{
    [XmlRoot(ElementName = "transform")]
    public class XmlTransform : XmlComponent
    {
        [XmlAttribute(AttributeName = "x")]
        public float X { get; set; }
        [XmlAttribute(AttributeName = "y")]
        public float Y { get; set; }
        [XmlAttribute(AttributeName = "z")]
        public float Z { get; set; }
        [XmlAttribute(AttributeName = "w")]
        public float W { get; set; }
        [XmlAttribute(AttributeName = "h")]
        public float H { get; set; }
        [XmlAttribute(AttributeName = "d")]
        public float D { get; set; }

        public override void CreateComponent(Gameobject gameobject)
        {
            Transform.Collection.Add(new Transform(X, Y, Z, W, H, D), gameobject);
        }
    }
}