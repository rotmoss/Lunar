using System;
using Lunar.Math;
using Lunar.Scenes;
using Lunar.Transforms;
using OpenGL;

namespace Lunar.Graphics
{
    public abstract class RenderData : IDisposable
    {
        public uint id;
        public VertexArray VertexArray { get => _vertexArray; }
        protected VertexArray _vertexArray;
        public Buffer PositionBuffer { get => _positionBuffer; }
        protected Buffer _positionBuffer;
        public ShaderProgram ShaderProgram { get => _shaderProgram; }
        protected ShaderProgram _shaderProgram;
        public string Layer { get => _layer; set => _layer = Window.RenderLayers.Contains(value) ? value : "default"; }
        private string _layer = "default";
        public bool Visible { get => _visible; set => _visible = value; }
        private bool _visible;

        public abstract void Render();
        public abstract void Dispose();

        public void TranslateBuffer(string attributeName, Vertex2f position, Vertex2f scale)
        {
            Vertex2f[] coords = new Vertex2f[(_positionBuffer.data.Length / _positionBuffer.size)];
            float[] data = new float[_positionBuffer.data.Length];

            for (int i = 0, j = 0; i < _positionBuffer.data.Length; j += 1, i += _positionBuffer.size) {
                coords[j] = new Vertex2f(_positionBuffer.data[i], _positionBuffer.data[i + 1]).Multiply(scale) + position;
            }

            for (int i = 0, j = 0; i < data.Length; i += _positionBuffer.size, j++) { 
                data[i] = coords[j].x; data[i + 1] = coords[j].y; 
            }

            _positionBuffer.UpdateBuffer(data);
        }

        public void TranslateBuffer(string attributeName, Transform transform)
        {
            Vertex2f pos = transform.position;
            Vertex2f scale = transform.scale;

            Vertex2f[] coords = new Vertex2f[(_positionBuffer.data.Length / _positionBuffer.size)];
            float[] data = new float[_positionBuffer.data.Length];

            for (int i = 0, j = 0; i < _positionBuffer.data.Length; j += 1, i += _positionBuffer.size)  {
                coords[j] = new Vertex2f(_positionBuffer.data[i], _positionBuffer.data[i + 1]).Multiply(scale) + pos;
            }

            for (int i = 0, j = 0; i < data.Length; i += _positionBuffer.size, j++) {
                data[i] = coords[j].x; data[i + 1] = coords[j].y;
            }

            _positionBuffer.UpdateBuffer(data);
        }
    }
}