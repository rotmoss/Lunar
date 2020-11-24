using System;
using System.Xml.Serialization;
using Lunar.Graphics;
using Lunar.Scripts;
using System.Linq;
using Lunar.Audio;
using Lunar.Physics;

namespace Lunar.IO
{
    [XmlRoot(ElementName = "gameobject")]
    public class XmlScene
    {
        [XmlElement("gameobject")]
        public XmlGameObject[] Gameobjects { get; set; }
    }

    [XmlRoot(ElementName = "entity")]
    public class XmlGameObject
    {
        [XmlArray("components")]
        [XmlArrayItem("transform", typeof(XmlTransform))]
        [XmlArrayItem("script", typeof(XmlScript))]
        [XmlArrayItem("sprite", typeof(XmlSprite))]
        [XmlArrayItem("text", typeof(XmlText))]
        [XmlArrayItem("animation", typeof(XmlAnimation))]
        [XmlArrayItem("collider", typeof(XmlCollider))]
        [XmlArrayItem("force", typeof(XmlForce))]
        [XmlArrayItem("sample", typeof(XmlSample))]
        public object[] Components;

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlAttribute(AttributeName = "enabled")]
        public bool Enabled { get; set; }
    }

    public abstract class XmlComponent
    {
        public abstract void CreateComponent(uint id);
    }

    [XmlRoot(ElementName = "transform")]
    public class XmlTransform : XmlComponent
    {
        [XmlAttribute(AttributeName = "x")]
        public float X { get; set; }
        [XmlAttribute(AttributeName = "y")]
        public float Y { get; set; }
        [XmlAttribute(AttributeName = "w")]
        public float W { get; set; }
        [XmlAttribute(AttributeName = "h")]
        public float H { get; set; }

        public override void CreateComponent(uint id)
        {
            Transform.AddComponent(id, new Transform(X, Y, W, H));
        }
    }

    [XmlRoot(ElementName = "script")]
    public class XmlScript : XmlComponent
    {
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }

        [XmlElement("var")]
        public XmlVar[] Vars { get; set; }

        public override void CreateComponent(uint id)
        {
            Script.AddComponent(id, new Script(File, Vars?.ToDictionary(x => x.Name, x => x.Value)));
        }
    }

    public abstract class XmlIRenderable : XmlComponent
    {
        [XmlAttribute(AttributeName = "vertexshader")]
        public string VertexShader { get; set; }
        [XmlAttribute(AttributeName = "fragmentshader")]
        public string FragmentShader { get; set; }
    }

    [XmlRoot(ElementName = "sprite")]
    public class XmlSprite : XmlIRenderable
    {
        [XmlElement(ElementName = "texture")]
        public string[] Textures { get; set; }

        public override void CreateComponent(uint id)
        {
            Sprite.AddComponent(id, new Sprite(VertexShader, FragmentShader, Textures));
        }
    }


    [XmlRoot(ElementName = "animation")]
    public class XmlAnimation : XmlIRenderable
    {
        [XmlAttribute(AttributeName = "framerate")]
        public double Framerate { get; set; }
        [XmlAttribute(AttributeName = "frameHeight")]
        public int FrameWidth { get; set; }
        [XmlAttribute(AttributeName = "frameWidth")]
        public int FrameHeight { get; set; }
        [XmlElement(ElementName = "texture")]
        public string[] Textures { get; set; }

        public override void CreateComponent(uint id)
        {
            Animation.AddComponent(id, new Animation(VertexShader, FragmentShader, Framerate, FrameWidth, FrameHeight, Textures));
        }
    }

    [XmlRoot(ElementName = "text")]
    public class XmlText : XmlIRenderable
    {
        [XmlAttribute(AttributeName = "font")]
        public string Font { get; set; }
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "size")]
        public int Size { get; set; }
        [XmlAttribute(AttributeName = "wrap")]
        public uint Wrap { get; set; }
        [XmlAttribute(AttributeName = "red")]
        public byte Red { get; set; }
        [XmlAttribute(AttributeName = "blue")]
        public byte Blue { get; set; }
        [XmlAttribute(AttributeName = "green")]
        public byte Green { get; set; }
        [XmlAttribute(AttributeName = "alpha")]
        public byte Alpha { get; set; }

        public override void CreateComponent(uint id)
        {
            Text.AddComponent(id, new Text(VertexShader, FragmentShader, Font, Message, Size, Wrap, Red, Green, Blue, Alpha));
        }
    }

    [XmlRoot(ElementName = "sample")]
    public class XmlSample : XmlComponent
    {
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }
        [XmlAttribute(AttributeName = "falloff")]
        public float Falloff { get; set; }
        [XmlAttribute(AttributeName = "pan")]
        public float Pan { get; set; }

        public override void CreateComponent(uint id)
        {
            Sample.AddComponent(id, new Sample(File, Falloff, Pan));
        }
    }

    [XmlRoot(ElementName = "force")]
    public class XmlForce : XmlComponent
    {
        [XmlAttribute(AttributeName = "friction")]
        public float Friction { get; set; }
        public override void CreateComponent(uint id)
        {
            Force.AddComponent(id, new Force(Friction));
        }
    }

    [XmlRoot(ElementName = "collider")]
    public class XmlCollider : XmlComponent
    {
        [XmlAttribute(AttributeName = "movable")]
        public bool Movable { get; set; }
        [XmlAttribute(AttributeName = "x")]
        public float X { get; set; }
        [XmlAttribute(AttributeName = "y")]
        public float Y { get; set; }
        [XmlAttribute(AttributeName = "w")]
        public float W { get; set; }
        [XmlAttribute(AttributeName = "h")]
        public float H { get; set; }

        public override void CreateComponent(uint id)
        {
            Collider.AddComponent(id, new Collider(new OpenGL.Vertex2f(X, Y), new OpenGL.Vertex2f(W, H), Movable));
        }
    }

    [XmlRoot(ElementName = "var")]
    public class XmlVar
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}
