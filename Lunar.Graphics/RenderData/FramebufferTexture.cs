using System;
using System.Collections.Generic;
using OpenGL;

namespace Lunar.Graphics
{
    public class FramebufferTexture : RenderData
    {
        public Framebuffer Framebuffer { get => _framebuffer; }
        private Framebuffer _framebuffer;

        public Buffer TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer _texCoordsBuffer;

        public Texture[] Textures { get => _textures; }
        private Texture[] _textures;

        public FramebufferTexture(int w, int h, int texCount, string vs, string fs)
        {
            _textures = new Texture[texCount];
            for (int i = 0; i < texCount; i++)
                Texture.CreateTexture(w, h, out _textures[i]);

            _framebuffer = new Framebuffer(_textures);
            ShaderProgram.CreateShader(vs, fs, out _shaderProgram);

            VertexArray.CreateVertexArray(_shaderProgram, out _vertexArray,
                new Buffer(new float[] { -1, -1, 1, -1, 1, 1, -1, 1 }, 2, "aPos"),
                new Buffer(new float[] { 0, 0, 1, 0, 1, 1, 0, 1 }, 2, "aTexCoord")
            );
        }

        public void OpenBuffer()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer.id);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void CloseBuffer()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void UpdateFrameSize(int w, int h)
        {
            for (int i = 0; i < _textures.Length; i++) {
                _textures[i].Dispose();
                Texture.CreateTexture(w, h, out _textures[i]);
            }

            Framebuffer.UpdateTextures(_textures);
        }

        public override void Render()
        {
            if (_shaderProgram == null) return;

            Gl.UseProgram(_shaderProgram.id);
            Gl.BindVertexArray(_vertexArray.id);

            Gl.Enable(EnableCap.Texture2d);

            for (int i = 0; i < _textures.Length; i++) {
                Gl.ActiveTexture(TextureUnit.Texture0 + i);
                Gl.BindTexture(TextureTarget.Texture2d, _textures[i].id);
            }

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);
        }

        public override void Dispose()
        {
            _vertexArray?.Dispose();
            for (int i = 0; i < _textures.Length; i++) _textures[i]?.Dispose();
            _positionBuffer.Dispose();
            _framebuffer.Dispose();
        }
    }
}
