using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Lunar.Graphics
{
    public class Sprite : RenderData
    {
        internal Texture texture;
        internal Texture[] textures;
        internal protected BufferObject texCoordsBuffer;

        public Sprite(uint Id, string textureFile, string vertexShader, string fragmentShader, out int w, out int h)
        {
            id = Id;
            textures = new Texture[1];
            Visible = true;

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram)) { Dispose(); w = h = 0; return; }
            if (!Texture.CreateTexture(textureFile, out w, out h, out textures[0])) { Dispose(); return; }

            positionBuffer = new BufferObject(new float[] { -w, -h, w, -h, w, h, -w, h }, 2, "aPos");
            texCoordsBuffer = new BufferObject(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray, positionBuffer, texCoordsBuffer)) { Dispose(); return; }

            SetSelectedTexture(0);

            _renderData.Add(this);
        }

        public override void Render()
        {
            Gl.Enable(EnableCap.Texture2d);

            if (!Visible || shaderProgram == null || vertexArray == null || texture == null) return;

            Gl.UseProgram(shaderProgram.id);
            Gl.BindVertexArray(vertexArray.id);
            Gl.BindTexture(TextureTarget.Texture2d, texture.id);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.Disable(EnableCap.Texture2d);
        }

        public void SetSelectedTexture(uint index) => texture = index < textures.Length ? textures[index] : texture;

        public override void Dispose()
        {
            vertexArray?.Dispose();
            texture?.Dispose();
            positionBuffer.Dispose();
            texCoordsBuffer.Dispose();
            _renderData.Remove(this);
        }
    }
}
