using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lunar.Scene;
using OpenGL;

namespace Lunar.Graphics
{
    public abstract class RenderData
    {
        public uint id;
        internal protected ShaderProgram shaderProgram;
        internal protected Texture texture;
        internal protected VertexArray vertexArray;
        internal protected BufferObject positionBuffer;
        internal protected BufferObject texCoordsBuffer;

        public bool Visible;

        public static List<RenderData> _renderData = new List<RenderData>();

        public static void TranslateBuffers(string attributeName)
        {
            foreach (RenderData g in _renderData)
            {
                Transform t = Transform.GetGlobalTransform(g.id);
                Vector2[] coords = new Vector2[(g.positionBuffer.data.Length / g.positionBuffer.size)];

                for (int i = 0, j = 0; i < g.positionBuffer.data.Length; j +=1, i += g.positionBuffer.size)
                {
                    coords[j] = new Vector2(g.positionBuffer.data[i], g.positionBuffer.data[i + 1]) * t.scale + t.position;
                }

                float[] data = new float[g.positionBuffer.data.Length];
                for (int i = 0, j = 0; i < data.Length; i += g.positionBuffer.size, j++)
                {
                    data[i] = coords[j].X;
                    data[i + 1] = coords[j].Y;
                }

                g.positionBuffer.UpdateBuffer(data);
            }
        }

        public static void Render()
        {
            Gl.Enable(EnableCap.Texture2d);
            for (int i = 0; i < _renderData.Count; i++)
            {
                if (!_renderData[i].Visible || _renderData[i].shaderProgram == null || _renderData[i].vertexArray == null || _renderData[i].texture == null) return;

                Gl.UseProgram(_renderData[i].shaderProgram.id);
                Gl.BindVertexArray(_renderData[i].vertexArray.id);
                Gl.BindTexture(TextureTarget.Texture2d, _renderData[i].texture.id);

                Gl.DrawArrays(PrimitiveType.Quads, 0, 4);
            }
            Gl.UseProgram(0);
            Gl.BindVertexArray(0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            Gl.Disable(EnableCap.Texture2d);
        }

        public void Dispose()
        {
            vertexArray?.Dispose();
            texture?.Dispose();
            positionBuffer.Dispose();
            texCoordsBuffer.Dispose();
        }

        public static void DisposeAll() 
        {
            foreach (RenderData graphicsObject in _renderData) {
                graphicsObject.Dispose();
            }
            ShaderProgram.DisposeShaders();
        }
    }
}