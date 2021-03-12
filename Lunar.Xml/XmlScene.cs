using System.Xml.Serialization;

namespace Lunar.Xml 
{
    [XmlRoot(ElementName = "gameobject")]
    public class XmlScene
    {
        [XmlElement("gameobject")]
        public XmlGameobject[] Gameobjects { get; set; }
    }
}