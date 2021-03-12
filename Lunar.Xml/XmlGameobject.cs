using System.Xml.Serialization;

namespace Lunar.Xml
{      
    [XmlRoot(ElementName = "entity")]
    public class XmlGameobject
    {
        [XmlArray("components")]
        [XmlArrayItem("transform", typeof(XmlTransform))]
        [XmlArrayItem("sprite", typeof(XmlSprite))]
        public object[] Components;

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlAttribute(AttributeName = "enabled")]
        public bool Enabled { get; set; }
    }
}