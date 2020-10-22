using System;
using System.Collections.Generic;
using System.Linq;
using OpenGL;

namespace Lunar.Graphics
{
    public class AnimatedSprite : RenderData
    {
        enum ScrollDirection { Vertical, Horizontal }

        internal Texture texture;
        internal Texture[] textures;
        internal protected BufferObject texCoordsBuffer;

        float _frameWidth;
        float _frameHeight;

        double _timeSinceLastFrame;
        double _timePerFrame;

        public AnimatedSprite(uint Id, string textureFile, string vertexShader, string fragmentShader, float frameRate, uint frameWidth, uint frameHeight, out int w, out int h)
        {
            id = Id;
            textures = new Texture[1];
            Visible = true;

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram)) { Dispose(); w = h = 0; return; }
            if (!Texture.CreateTexture(textureFile, out w, out h, out textures[0])) { Dispose(); return; }

            _frameWidth = frameWidth / (float)w;
            _frameHeight = frameHeight / (float)h;

            positionBuffer = new BufferObject(new float[] { -frameWidth, -frameHeight, frameWidth, -frameHeight, frameWidth, frameHeight, -frameWidth, frameHeight }, 2, "aPos");
            texCoordsBuffer = new BufferObject(new float[] { 0, _frameHeight, _frameWidth, _frameHeight, _frameWidth, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray, positionBuffer, texCoordsBuffer)) { Dispose(); return; }

            SetSelectedTexture(0);
            _timePerFrame = 1.0d / frameRate;

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

        public void NextFrame(double frameTime)
        {
            if (_timeSinceLastFrame < _timePerFrame) { _timeSinceLastFrame += frameTime; return; }
            else { _timeSinceLastFrame = 0; }

            float[] data = texCoordsBuffer.data;

            if (data[0] + _frameWidth >= 1) ResetTextureCoords(ref data, ScrollDirection.Horizontal);
            else ScrollTextureCoords(ref data, ScrollDirection.Horizontal);

            if (data[1] + _frameHeight >= 1) ResetTextureCoords(ref data, ScrollDirection.Vertical);
            else ScrollTextureCoords(ref data, ScrollDirection.Vertical);

            texCoordsBuffer.UpdateBuffer(data);
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
            foreach(AnimatedSprite r in _renderData.Where(x => x.GetType() == typeof(AnimatedSprite)))
                r.NextFrame(frameTime);            
        }
    }
}
