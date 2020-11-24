﻿using OpenGL;
using Lunar.Graphics;

namespace Lunar
{
    public class FramebufferTexture : GraphicsComponent
    {
        public Framebuffer Framebuffer { get => _framebuffer; }
        private Framebuffer _framebuffer;

        public Buffer<float> TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer<float> _texCoordsBuffer;

        public Texture[] Textures { get => _textures; }
        private Texture[] _textures;

        public FramebufferTexture(string vs, string fs, int w, int h, int texCount) : base(vs, fs)
        {
            _textures = new Texture[texCount];

            for (int i = 0; i < texCount; i++)
                Texture.CreateTexture(w, h, out _textures[i]);

            _framebuffer = new Framebuffer(_textures);

            _positionBuffer = new Buffer<float>(new float[] { -1, -1, 1, -1, 1, 1, -1, 1 }, 2, "aPos");
            _texCoordsBuffer = new Buffer<float>(new float[] { 0, 0, 1, 0, 1, 1, 0, 1 }, 2, "aTexCoord");

            _vertexArray.AddBuffer(_positionBuffer, _texCoordsBuffer);
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
                _textures[i].Resize(w, h);
            }
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

        public override void DisposeChild()
        {
            for (int i = 0; i < _textures.Length; i++) 
                _textures[i]?.Dispose();

            _vertexArray?.Dispose();
            _positionBuffer.Dispose();
            _framebuffer.Dispose();
        }
    }
}