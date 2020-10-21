using System;
using OpenGL;

namespace Lunar.Graphics
{
    public class Text : RenderData
    {
        internal Texture texture;
        internal protected BufferObject texCoordsBuffer;
        public Text(string fontFile, string message, int size, uint wrapped, byte r, byte g, byte b, byte a, string vertexShader, string fragmentShader, out int w, out int h)
        {
            Visible = true;

            if(!ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram)) { Dispose(); w = h = 0; return; }
            if (!Texture.CreateText(fontFile, message, size, wrapped, r, g, b, a, out w, out h, out texture)){ Dispose(); return; }

            positionBuffer = new BufferObject(new float[] { -w, -h, 0, w, -h, 0, w, h, 0, -w, h, 0 }, 3, "aPos");
            texCoordsBuffer = new BufferObject(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray, positionBuffer, texCoordsBuffer)){ Dispose(); return; }

            _renderData.Add(this);
        }

        public void UpdateText(string fontFile, string message, int size, uint wrapped, byte r, byte g, byte b, byte a, out int w, out int h)
        {
            texture.Dispose();
            Texture.CreateText(fontFile, message, size, wrapped, r, g, b, a, out w, out h, out texture);
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
