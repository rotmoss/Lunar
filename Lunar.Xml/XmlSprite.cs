using System.Xml;
using System.Xml.Serialization;
using Lunar.OpenGL;
using Lunar.ECS;

namespace Lunar.Xml 
{
    [XmlRoot(ElementName = "sprite")]
    public class XmlSprite : XmlComponent
    {
        [XmlAttribute(AttributeName = "shader")]
        public string Shader { get; set; }
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
}