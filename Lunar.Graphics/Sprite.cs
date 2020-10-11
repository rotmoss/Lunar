using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Graphics
{
    public class Sprite : RenderData
    {
        internal Texture[] textures;
        public Sprite(uint Id, string textureFile, string vertexShader, string fragmentShader, out int w, out int h)
        {
            id = Id;
            textures = new Texture[1];
            Visible = true;

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram)) { Dispose(); w = h = 0; return; }
            if (!Texture.CreateTexture(textureFile, out w, out h, out textures[0])) { Dispose(); return; }

            positionBuffer = new BufferObject(new float[] { -w, -h, 0, w, -h, 0, w, h, 0, -w, h, 0 }, 3, "aPos");
            texCoordsBuffer = new BufferObject(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray, positionBuffer, texCoordsBuffer)) { Dispose(); return; }

            SetSelectedTexture(0);

            _renderData.Add(this);
        }


        public void SetSelectedTexture(uint index)
        {
            if (index < textures.Length) { texture = textures[index]; }
        }
    }
}
