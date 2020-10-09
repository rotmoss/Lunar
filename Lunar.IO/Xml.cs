using System;
using System.Xml.Serialization;

namespace Lunar.IO
{
    [XmlRoot(ElementName = "var")]
    public class XmlElementVar
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "script")]
    public class XmlElementScript
    {
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }

        [XmlElement("var")]
        public XmlElementVar[] Vars { get; set; }
    }

    [XmlRoot(ElementName = "entity")]
    public class XmlElementEntity
    {
        [XmlElement("script")]
        public XmlElementScript Script { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlAttribute(AttributeName = "enabled")]
        public bool Enabled { get; set; }
    }
    [XmlRoot(ElementName = "entity")]
    public class XmlElementScene
    {
        [XmlElement("entity")]
        public XmlElementEntity[] entities { get; set; }
    }
}
