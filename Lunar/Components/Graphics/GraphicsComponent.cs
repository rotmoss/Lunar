using Lunar.Math;
using Lunar.Graphics;
using System.Collections.Generic;
using OpenGL;

namespace Lunar
{
    public abstract class GraphicsComponent : Component<GraphicsComponent>
    {
        public ShaderProgram ShaderProgram { get => _shaderProgram; }
        protected ShaderProgram _shaderProgram;

        public VertexArray VertexArray { get => _vertexArray; }
        protected VertexArray _vertexArray;

        public Buffer<float> PositionBuffer { get => _positionBuffer; }
        protected Buffer<float> _positionBuffer;

        public string Layer { 
            get => _layer; 
            set => _layer = Window.RenderLayers.Contains(value.ToLower()) ? value.ToLower() : "default"; 
        }
        private string _layer;

        public GraphicsComponent(string vertexShader, string fragmentShader) : base()
        {
            _layer = "default";

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out _shaderProgram)) { Dispose(); return; }
            if (!VertexArray.CreateVertexArray(_shaderProgram, out _vertexArray)) { Dispose(); return; }
        }

        public static List<GraphicsComponent> GetRenderData(string layer)
        {
            List<GraphicsComponent> list = new List<GraphicsComponent>();
            for (int i = 0; i < _components.Count; i++)
                if (_components[i].Layer== layer) list.Add(_components[i]);
            return list;
        }

        public static void TranslateBuffers()
        {
            foreach (GraphicsComponent component in _components)
                component.TranslateBuffer(Transform.GetGlobalTransform(component.Id));
        }

        public void TranslateBuffer(Transform transform)
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

        public abstract void Render();
    }
}