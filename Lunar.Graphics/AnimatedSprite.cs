using System;
using System.Collections.Generic;
using System.Linq;
using Lunar.Stopwatch;

namespace Lunar.Graphics
{
    public class AnimatedSprite : RenderData
    {
        double time;
        double frameTime;
        float frame_w, frame_h;
        internal Texture[] textures;
        public AnimatedSprite(uint Id, string textureFile, uint frameWidth, uint frameHeight, double framerate, string vertexShader, string fragmentShader, out int w, out int h)
        {
            id = Id;
            textures = new Texture[1];
            Visible = true;

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram)) { Dispose(); w = h = 0; return; }
            if (!Texture.CreateTexture(textureFile, out w, out h, out textures[0])) { Dispose(); return; }
            
            frame_w = frameWidth / (float)w;
            frame_h = frameHeight / (float)h;

            positionBuffer = new BufferObject(new float[] { -frameWidth, -frameHeight, 0, frameWidth, -frameHeight, 0, frameWidth, frameHeight, 0, -frameWidth, frameHeight, 0 }, 3, "aPos");
            texCoordsBuffer = new BufferObject(new float[] { 0, frame_h, frame_w, frame_h, frame_w, 0, 0, 0 }, 2, "aTexCoord");

            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray, positionBuffer, texCoordsBuffer)) { Dispose(); return; }

            SetSelectedTexture(0);
            frameTime = 1.0d / framerate;

            _renderData.Add(this);
        }

        public void NextFrame()
        {
            float[] data = texCoordsBuffer.data;

            if (data[0] + frame_w >= 1)
            {
                if (data[1] + frame_h >= 1)
                {
                    data[1] = data[3] = frame_h;
                    data[5] = data[7] = 0;
                }
                else
                {
                    for (int i = 1; i < data.Length; i += 2)
                        data[i] += frame_h;
                }

                data[2] = data[4] = frame_w;
                data[0] = data[6] = 0;
            }
            else
            {
                for (int i = 0; i < data.Length; i += 2)
                    data[i] += frame_w;
            }

            texCoordsBuffer.UpdateBuffer(data);
        }

        public static void Animate()
        {
            foreach(AnimatedSprite r in _renderData.Where(x => x.GetType() == typeof(AnimatedSprite)))
            {
                if (r.time > r.frameTime)
                {
                    r.NextFrame();
                    r.time = 0;
                }
                else
                {
                    r.time += Time.FrameTime;
                }
            }
        }

        public void SetSelectedTexture(uint index)
        {
            if (index < textures.Length) { texture = textures[index]; }
        }
    }
}
