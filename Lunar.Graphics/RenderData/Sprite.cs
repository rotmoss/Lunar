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
        public Texture[] Textures { get => _textures; }
        private Texture[] _textures;

        public Buffer TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer _texCoordsBuffer;

        public Sprite(uint Id, string[] textureFiles, string vertexShader, string fragmentShader, out int w, out int h)
        {
            id = Id;
            Visible = true;
            w = h = 0;

            _textures = new Texture[textureFiles.Length];

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out _shaderProgram)) { Dispose(); w = h = 0; return; }
            for (int i = 0; i < textureFiles.Length; i++)
                if (!Texture.CreateTextureFromFile(textureFiles[i], out w, out h, out _textures[i])) { Dispose(); return; }

            _positionBuffer = new Buffer(new double[] { -w, -h, w, -h, w, h, -w, h }, 2, "aPos");
            _texCoordsBuffer = new Buffer(new double[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(_shaderProgram, out _vertexArray, _positionBuffer, _texCoordsBuffer)) { Dispose(); return; }

            Window.AddRenderData(this);
        }

        public override void Render()
        {
            Gl.Enable(EnableCap.Texture2d);

            if (!Visible || _shaderProgram == null || _vertexArray == null || _textures == null) return;

            Gl.UseProgram(_shaderProgram.id);
            Gl.BindVertexArray(_vertexArray.id);

            for (int i = 0; i < _textures.Length; i++) {
                Gl.ActiveTexture(TextureUnit.Texture0 + i);
                Gl.BindTexture(TextureTarget.Texture2d, _textures[i].id);
            }

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.Disable(EnableCap.Texture2d);
        }

        public override void Dispose()
        {
            _vertexArray?.Dispose();
            for(int i = 0; i < _textures.Length; i++) _textures[i]?.Dispose();
            _positionBuffer.Dispose();
            _texCoordsBuffer.Dispose();
            Window.RemoveRenderData(this);
        }
    }
}
