using System.Xml.Serialization;
using Lunar.ECS;
using Lunar.OpenGL;

namespace Lunar
{
    [XmlRoot(ElementName = "gameobject")]
    public class XmlScene
    {
        [XmlElement("gameobject")]
        public XmlGameobject[] Gameobjects { get; set; }
    }

    [XmlRoot(ElementName = "entity")]
    public class XmlGameobject
    {
        [XmlArray("components")]
        [XmlArrayItem("transform", typeof(XmlTransform))]
        //[XmlArrayItem("script", typeof(XmlScript))]
        [XmlArrayItem("sprite", typeof(XmlSprite))]
        //[XmlArrayItem("text", typeof(XmlText))]
        //[XmlArrayItem("animation", typeof(XmlAnimation))]
        //[XmlArrayItem("collider", typeof(XmlCollider))]
        //[XmlArrayItem("force", typeof(XmlForce))]
        //[XmlArrayItem("sample", typeof(XmlSample))]
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
        public abstract void CreateComponent(Gameobject gameobject);
    }

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

/*
    [XmlRoot(ElementName = "script")]
    public class XmlScript : XmlComponent
    {
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }

        [XmlElement("var")]
        public XmlVar[] Vars { get; set; }

        public override void CreateComponent(Gameobject gameobject)
        {
            Script.Collection.AddEntry(gameobject, new Script(File, Vars?.ToDictionary(x => x.Name, x => x.Value)));
        }
    }
*/

    public abstract class XmlIRenderable : XmlComponent
    {
        [XmlAttribute(AttributeName = "shader")]
        public string Shader { get; set; }
    }

    [XmlRoot(ElementName = "sprite")]
    public class XmlSprite : XmlIRenderable
    {
        [XmlElement(ElementName = "texture")]
        public string[] Textures { get; set; }

        public override void CreateComponent(Gameobject gameobject)
        {
            OpenGL.Texture[] textures = new Texture[Textures.Length];
            for(int i = 0; i < Textures.Length; i++) 
                textures[i] = Texture.LoadImage(Textures[i]);

            Sprite.Collection.Add(new Sprite(Material.CreateMaterial(ShaderProgram.CreateShaderProgram(Shader), textures)), gameobject);
        }
    }

/*
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

        public override void CreateComponent(Gameobject gameobject)
        {
            Animation.Collection.AddEntry(gameobject, new Animation(VertexShader, FragmentShader, Framerate, FrameWidth, FrameHeight, Textures));
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

        public override void CreateComponent(Gameobject gameobject)
        {
            Text.Collection.AddEntry(gameobject, new Text(VertexShader, FragmentShader, Font, Message, Size, Wrap, Red, Green, Blue, Alpha));
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

        public override void CreateComponent(Gameobject gameobject)
        {
            Sample.Collection.AddEntry(gameobject, new Sample(File, Falloff, Pan));
        }
    }

    [XmlRoot(ElementName = "force")]
    public class XmlForce : XmlComponent
    {
        [XmlAttribute(AttributeName = "friction")]
        public float Friction { get; set; }

        public override void CreateComponent(Gameobject gameobject)
        {
            Force.Collection.AddEntry(gameobject, new Force(Friction));
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

        public override void CreateComponent(Gameobject gameobject)
        {
            Collider.Collection.AddEntry(gameobject, new Collider(new OpenGL.Vertex2f(X, Y), new OpenGL.Vertex2f(W, H), Movable));
        }
    }
*/

    [XmlRoot(ElementName = "var")]
    public class XmlVar
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}