using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar
{
    public struct GraphicsObject
    {
        public uint VertexArrayObject;
        public uint SelectedTexture;
        public uint ShaderProgram;
        public uint[] Textures;
        public BufferObject Position;
        public BufferObject TexCoord;
        public bool Visible;
    }
    public partial class GraphicsController
    {
        private static GraphicsController instance = null;
        public static GraphicsController Instance { get { instance = instance == null ? new GraphicsController() : instance; return instance; } }
        public Dictionary<uint, GraphicsObject> _graphicsObjects;

        private GraphicsController()
        {
            _graphicsObjects = new Dictionary<uint, GraphicsObject>();
            _shaderPrograms = new List<ShaderObject>();
            _vertexShaders = new Dictionary<string, uint>();
            _fragmentShaders = new Dictionary<string, uint>();
        }

        public void CreateSprite(uint id, string file, string vertexShader, string fragmentShader, out int w, out int h)
        {
            if (!_graphicsObjects.ContainsKey(id)) _graphicsObjects.Add(id, new GraphicsObject());
            GraphicsObject graphicsObject = _graphicsObjects[id];

            graphicsObject.ShaderProgram = CreateShader(vertexShader, fragmentShader);

            graphicsObject.Textures = graphicsObject.Textures.Add(CreateTexture(file, out w, out h));
            graphicsObject.SelectedTexture = graphicsObject.Textures[0];

            graphicsObject.Position = CreateBuffer(GenVertexSquare(w, h), "aPos", 3);
            graphicsObject.TexCoord = CreateBuffer(GenTextureSquare(0, 0, 1, 1), "aTexCoord", 2);
            graphicsObject.VertexArrayObject = CreateVertexArray(graphicsObject.ShaderProgram, graphicsObject.Position, graphicsObject.TexCoord);
            graphicsObject.Visible = true;
            _graphicsObjects[id] = graphicsObject;
        }
    
        public void CreateText(uint id, string font, string text, int size, Color color, uint wrapper, string vertexShader, string fragmentShader, out int w, out int h)
        {
            if (!_graphicsObjects.ContainsKey(id)) _graphicsObjects.Add(id, new GraphicsObject());
            GraphicsObject graphicsObject = _graphicsObjects[id];

            graphicsObject.ShaderProgram = CreateShader(vertexShader, fragmentShader);

            graphicsObject.Textures = graphicsObject.Textures.Add(CreateText(font, text, size, color, wrapper, out w, out h));
            graphicsObject.SelectedTexture = graphicsObject.Textures[0];

            graphicsObject.Position = CreateBuffer(GenVertexSquare(w, h), "aPos", 3);
            graphicsObject.TexCoord = CreateBuffer(GenTextureSquare(0, 0, w, h), "aTexCoord", 2);
            graphicsObject.VertexArrayObject = CreateVertexArray(graphicsObject.ShaderProgram, graphicsObject.Position, graphicsObject.TexCoord);

            graphicsObject.Visible = true;
            _graphicsObjects[id] = graphicsObject;
        }
        public bool GetEntityVisibility(uint id) => _graphicsObjects.ContainsKey(id) ? _graphicsObjects[id].Visible : false;
        public void SetEntityVisibility(uint id, bool value) { if (_graphicsObjects.ContainsKey(id)) { GraphicsObject x = _graphicsObjects[id]; x.Visible = value; _graphicsObjects[id] = x; } }

        internal void Dispose() { foreach (uint id in _graphicsObjects.Keys) Dispose(id); DisposeShaders(); }

        public void Dispose(uint id)
        {
            GraphicsObject graphicsObject = _graphicsObjects[id];
            Gl.DeleteVertexArrays(graphicsObject.VertexArrayObject);
            Gl.DeleteBuffers(_graphicsObjects[id].Position.id);
            Gl.DeleteBuffers(_graphicsObjects[id].TexCoord.id);
            Array.ForEach(graphicsObject.Textures, x => Gl.DeleteTextures(x));
        }
    }
}
