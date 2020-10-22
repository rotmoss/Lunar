using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lunar.Scenes;
using OpenGL;

namespace Lunar.Graphics
{
    public abstract class RenderData : IDisposable
    {
        public uint id;
        public VertexArray VertexArray { get => vertexArray; }
        internal protected VertexArray vertexArray;
        public BufferObject PositionBuffer { get => positionBuffer; }
        internal protected BufferObject positionBuffer;
        public ShaderProgram ShaderProgram { get => shaderProgram; }
        internal protected ShaderProgram shaderProgram;
        public string Layer { get => _layer; set => _layer = _layers.Contains(value) ? value : "default"; }
        private string _layer = "default";
        public bool Visible { get => _visible; set => _visible = value; }
        private bool _visible;

        public static List<string> _layers = new List<string>(new string[] { "default" });
        public static List<RenderData> _renderData = new List<RenderData>();

        public static void TranslateBuffers(string attributeName)
        {
            foreach (RenderData g in _renderData)
            {
                Transform t = Transform.GetGlobalTransform(g.id);
                Vector2[] coords = new Vector2[(g.positionBuffer.data.Length / g.positionBuffer.size)];

                for (int i = 0, j = 0; i < g.positionBuffer.data.Length; j +=1, i += g.positionBuffer.size)
                    coords[j] = new Vector2(g.positionBuffer.data[i], g.positionBuffer.data[i + 1]) * t.scale + t.position;
                

                float[] data = new float[g.positionBuffer.data.Length];
                for (int i = 0, j = 0; i < data.Length; i += g.positionBuffer.size, j++) {
                    data[i] = coords[j].X; data[i + 1] = coords[j].Y;
                }

                g.positionBuffer.UpdateBuffer(data);
            }
        }
        public static void SetLineWidth(int value) => Gl.LineWidth(value);
        public static void AddLayer(string value, int index) => _layers.Insert(index, value);
        public static void AddLayers(params string[] value) => _layers.AddRange(value);

        public abstract void Render();
        public static void RenderAll()
        {
            foreach (string layer in _layers)
                foreach (RenderData renderData in _renderData.Where(x => x._layer == layer))
                    renderData.Render();


            Gl.UseProgram(0);
            Gl.BindVertexArray(0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }

        public abstract void Dispose();

        public static void DisposeAll() 
        {
            while(_renderData.Count > 0)
            {
                _renderData[0].Dispose();
            }

            ShaderProgram.DisposeShaders();
        }
    }
}