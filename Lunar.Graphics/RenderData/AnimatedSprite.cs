using System;
using System.Collections.Generic;
using System.Linq;
using OpenGL;

namespace Lunar.Graphics
{
    public class AnimatedSprite : RenderData
    {
        enum ScrollDirection { Vertical, Horizontal }

        public Texture[] Textures { get => _textures; }
        private Texture[] _textures;
        public Buffer TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer _texCoordsBuffer;

        float _frameWidth;
        float _frameHeight;

        double _timeSinceLastFrame;
        double _timePerFrame;

        private static List<AnimatedSprite> _animations = new List<AnimatedSprite>();

        public AnimatedSprite(uint Id, string[] textureFiles, string vertexShader, string fragmentShader, double frameRate, uint frameWidth, uint frameHeight, out int w, out int h)
        {
            id = Id;
            Visible = true;
            w = h = 0;

            _textures = new Texture[textureFiles.Length];
            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out _shaderProgram)) { Dispose(); w = h = 0; return; }
            for (int i = 0; i < textureFiles.Length; i++)
                if (!Texture.CreateTextureFromFile(textureFiles[i], out w, out h, out _textures[i])) { Dispose(); return; }

            _frameWidth = frameWidth / (float)w;
            _frameHeight = frameHeight / (float)h;

            _positionBuffer = new Buffer(new float[] { -frameWidth, -frameHeight, frameWidth, -frameHeight, frameWidth, frameHeight, -frameWidth, frameHeight }, 2, "aPos");
            _texCoordsBuffer = new Buffer(new float[] { 0, _frameHeight, _frameWidth, _frameHeight, _frameWidth, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(_shaderProgram, out _vertexArray, _positionBuffer, _texCoordsBuffer)) { Dispose(); return; }

            _timePerFrame = 1.0d / frameRate;

            Window.AddRenderData(this);
            _animations.Add(this);
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
            for (int i = 0; i < _textures.Length; i++) _textures[i]?.Dispose();
            _positionBuffer.Dispose();
            _texCoordsBuffer.Dispose();
            Window.RemoveRenderData(this);
            _animations.Remove(this);
        }

        public void NextFrame(double frameTime)
        {
            if (_timeSinceLastFrame < _timePerFrame) { _timeSinceLastFrame += frameTime; return; }
            else { _timeSinceLastFrame = 0; }

            float[] data = _texCoordsBuffer.data;

            if (data[0] + _frameWidth >= 1) ResetTextureCoords(ref data, ScrollDirection.Horizontal);
            else ScrollTextureCoords(ref data, ScrollDirection.Horizontal);

            if (data[1] + _frameHeight >= 1) ResetTextureCoords(ref data, ScrollDirection.Vertical);
            else ScrollTextureCoords(ref data, ScrollDirection.Vertical);

            _texCoordsBuffer.UpdateBuffer(data);
        }

        private void ScrollTextureCoords(ref float[] data, ScrollDirection scrollDirection)
        {
            for (int i = scrollDirection == ScrollDirection.Horizontal ? 0 : 1; i < data.Length; i += 2)
                data[i] += scrollDirection == ScrollDirection.Horizontal ? _frameWidth : _frameHeight;
        }

        private void ResetTextureCoords(ref float[] data, ScrollDirection scrollDirection)
        {
            data = scrollDirection == ScrollDirection.Horizontal ? 
                new float[] { 0, data[1], _frameWidth, data[3], _frameWidth, data[5], 0, data[7] } :
                new float[] { data[0], _frameHeight, data[3], _frameHeight, data[4], 0, data[6], 0 } ;
        }

        public static void Animate(double frameTime)
        {
            foreach(AnimatedSprite a in _animations)
                a.NextFrame(frameTime);            
        }
    }
}
