using System;
using OpenGL;

namespace Lunar.Graphics
{
    public class TextSprite : RenderData
    {
        public Texture Texture { get => _texture; }
        private Texture _texture;
        public Buffer TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer _texCoordsBuffer;

        public TextSprite(string fontFile, string message, int size, uint wrapped, byte r, byte g, byte b, byte a, string vertexShader, string fragmentShader, out int w, out int h)
        {
            Visible = true;

            if(!ShaderProgram.CreateShader(vertexShader, fragmentShader, out _shaderProgram)) { Dispose(); w = h = 0; return; }
            if (!Texture.CreateTextureFromText(fontFile, message, size, wrapped, r, g, b, a, out w, out h, out _texture)){ Dispose(); return; }

            _positionBuffer = new Buffer(new double[] { -w, -h, w, -h, w, h, -w, h }, 2, "aPos");
            _texCoordsBuffer = new Buffer(new double[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(_shaderProgram, out _vertexArray, _positionBuffer, _texCoordsBuffer)){ Dispose(); return; }

            Window.AddRenderData(this);
        }

        public void UpdateText(string fontFile, string message, int size, uint wrapped, byte r, byte g, byte b, byte a, out int w, out int h)
        {
            _texture.Dispose();
            Texture.CreateTextureFromText(fontFile, message, size, wrapped, r, g, b, a, out w, out h, out _texture);
        }

        public override void Render()
        {
            Gl.Enable(EnableCap.Texture2d);

            if (!Visible || _shaderProgram == null || _vertexArray == null || _texture == null) return;

            Gl.UseProgram(_shaderProgram.id);
            Gl.BindVertexArray(_vertexArray.id);
            Gl.BindTexture(TextureTarget.Texture2d, _texture.id);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.Disable(EnableCap.Texture2d);
        }

        public override void Dispose()
        {
            _vertexArray?.Dispose();
            _texture?.Dispose();
            _positionBuffer.Dispose();
            _texCoordsBuffer.Dispose();
            Window.RemoveRenderData(this);
        }
    }
}
