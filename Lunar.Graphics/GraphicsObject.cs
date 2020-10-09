using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lunar.Scene;
using OpenGL;

namespace Lunar.Graphics
{
    public class GraphicsObject
    {
        public uint id;
        internal ShaderProgram shaderProgram;
        internal Texture[] textures;
        internal Texture selectedTexture;
        internal VertexArray vertexArray;
        
        public bool Visible;

        public static List<GraphicsObject> _graphicsObjects = new List<GraphicsObject>();

        public GraphicsObject(uint Id,string textureFile, string vertexShader, string fragmentShader, out int w, out int h)
        {
            id = Id;
            w = h = 0;
            textures = new Texture[1];
            Visible = true;

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram)) { Dispose(); return; }
            if (!Texture.CreateTexture(textureFile, out w, out h, out textures[0])) { Dispose(); return; }
            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray,
                new BufferObject(new float[] { -w, -h, 0, w, -h, 0, w, h, 0, -w, h, 0 }, 3, "aPos"),
                new BufferObject(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord")
            )) { Dispose(); return; }

            SetSelectedTexture(0);

            _graphicsObjects.Add(this);
        }

        public GraphicsObject(string fontFile, string message, int size, uint wrapped, byte r, byte g, byte b, byte a, string vertexShader, string fragmentShader, out int w, out int h)
        {
            textures = new Texture[1];
            Visible = true;

            ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram);
            Texture.CreateText(fontFile, message, size, wrapped, r, g, b, a, out w, out h, out textures[0]);
            VertexArray.CreateVertexArray(shaderProgram, out vertexArray, 
                new BufferObject(new float[] { -w, -h, 0, w, -h, 0, w, h, 0, -w, h, 0 }, 3, "aPos"), 
                new BufferObject(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord")
            );

            _graphicsObjects.Add(this);
        }

        public static void TranslateBuffers(string attributeName)
        {
            foreach (GraphicsObject g in _graphicsObjects)
            {
                Transform t = Transform.GetGlobalTransform(g.id);
                BufferObject bufferObject = g.vertexArray.buffers.Where(x => x.name == attributeName).FirstOrDefault();
                Vector2[] coords = new Vector2[(bufferObject.data.Length / bufferObject.size)];

                for (int i = 0, j = 0; i < bufferObject.data.Length; j +=1, i += bufferObject.size)
                {
                    coords[j] = new Vector2(bufferObject.data[i], bufferObject.data[i + 1]) * t.scale + t.position;
                }

                float[] data = new float[bufferObject.data.Length];
                for (int i = 0, j = 0; i < data.Length; i += bufferObject.size, j++)
                {
                    data[i] = coords[j].X;
                    data[i + 1] = coords[j].Y;
                }

                bufferObject.UpdateBuffer(data);
            }
        }

        public void SetSelectedTexture(uint index)
        {
            if(index < textures.Length) { selectedTexture = textures[index]; }
        }

        public static void Render()
        {
            Gl.Enable(EnableCap.Texture2d);
            for (int i = 0; i < _graphicsObjects.Count; i++)
            {
                if (!_graphicsObjects[i].Visible || _graphicsObjects[i].shaderProgram == null || _graphicsObjects[i].vertexArray == null || _graphicsObjects[i].selectedTexture == null) return;

                Gl.UseProgram(_graphicsObjects[i].shaderProgram.id);
                Gl.BindVertexArray(_graphicsObjects[i].vertexArray.id);
                Gl.BindTexture(TextureTarget.Texture2d, _graphicsObjects[i].selectedTexture.id);

                Gl.DrawArrays(PrimitiveType.Quads, 0, 4);
            }
            Gl.UseProgram(0);
            Gl.BindVertexArray(0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            Gl.Disable(EnableCap.Texture2d);
        }

        public void Dispose()
        {
            shaderProgram?.Dispose();
            vertexArray?.Dispose();

            foreach (Texture texture in textures) {
                texture?.Dispose();
            }
        }

        public static void DisposeAll() 
        {
            foreach (GraphicsObject graphicsObject in _graphicsObjects) {
                graphicsObject.Dispose();
            }
        }
    }
}