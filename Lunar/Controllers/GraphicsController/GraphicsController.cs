using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenGL;

namespace Lunar
{
    public partial class GraphicsController
    {
        private static GraphicsController instance = null;
        public static GraphicsController Instance { get { instance = instance == null ? new GraphicsController() : instance; return instance; } }
        private GraphicsController()
        {
            _shaders = new Dictionary<uint, uint>();
            _textures = new Dictionary<uint, List<uint>>();
            _selectedTexture = new Dictionary<uint, int>();
            _vertexArray = new Dictionary<uint, uint>();
            _buffers = new Dictionary<uint, List<Buffer>>();
            _graphicsTransform = new Dictionary<uint, Transform>();
        }

        public void AddShader(uint id, string vs, string fs)
        {
            if (_shaders.ContainsKey(id)) { Gl.DeleteProgram(_shaders[id]); _shaders.Remove(id); }
            _shaders.Add(id, CreateShader(vs, fs));
        }

        public void AddTexture(uint id, string texture, out int w, out int h)
        {
            if (!_textures.ContainsKey(id)) { _textures.Add(id, new List<uint>()); }
            _textures[id].Add(CreateTexture(texture, out w, out h));
            if (!_selectedTexture.ContainsKey(id)) { _selectedTexture.Add(id, 0); }
        }

        public void AddText(uint id, string font, string text, int size, Color color, uint wrapper, out int w, out int h)
        {
            if (!_textures.ContainsKey(id)) { _textures.Add(id, new List<uint>()); }
            _textures[id].Add(CreateText(font, text, size, color, wrapper, out w, out h));
            if (!_selectedTexture.ContainsKey(id)) { _selectedTexture.Add(id, 0); }
        }

        public void AddBuffer(uint id, int w, int h, int textureX = 0, int textureY = 0, int textureW = 1, int textureH = 1)
        {
            if (_buffers.ContainsKey(id)) { _buffers[id].ForEach(x => Gl.DeleteProgram(x.id)); _buffers.Remove(id); }
            _buffers.Add(id, new List<Buffer>());

            _buffers[id].Add(CreateBuffer(GenVertexSquare(w, h), "aPos", 3));
            _buffers[id].Add(CreateBuffer(GenTextureSquare(textureX, textureY, textureW, textureH), "aTexCoord", 2));

            if (_vertexArray.ContainsKey(id)) { Gl.DeleteVertexArrays(_vertexArray[id]); _vertexArray.Remove(id); }
            _vertexArray.Add(id, CreateVertexArray(_shaders[id], _buffers[id]));
        }

        public void CreateSprite(uint id, string file, string vertexShader, string fragmentShader, out int w, out int h)
        {
            AddShader(id, vertexShader, fragmentShader);
            AddTexture(id, file, out w, out h);
            AddBuffer(id, w, h);
            AddTransform(id);
        }
    
        public void CreateText(uint id, string font, string text, int size, Color color, uint wrapper, string vertexShader, string fragmentShader, out int w, out int h)
        {
            AddShader(id, vertexShader, fragmentShader);
            AddText(id, font, text, size, color, wrapper, out w, out h);
            AddBuffer(id, w, h);
            AddTransform(id);
        }
    
        /// <summary> Uses the graphics transform aswell as the entity transform to translate the vertexbuffer linked to the spcified id. </summary>
        /// <param name="transforms"> A dictionary containing all entity ids and their respective entity transform </param>
        internal void TranslateBuffers(Dictionary<uint, Transform> transforms)
        {
            foreach (KeyValuePair<uint, Transform> pair in transforms)
            {
                if (_graphicsTransform.ContainsKey(pair.Key) && _buffers.ContainsKey(pair.Key)) {
                    TranslateBuffer(pair.Key, pair.Value + _graphicsTransform[pair.Key]);
                }               
            }
        }

        internal void TranslateBuffer(uint id, Transform transform)
        {
            Buffer buffer = _buffers[id].Where(x => x.name == "aPos").FirstOrDefault();

            float[] bufferData = new float[buffer.data.Length];
            for (int i = 0; i < bufferData.Length; i++) bufferData[i] = buffer.data[i];

            bufferData[0] = (bufferData[0] + transform.position.X) * transform.scale.X;
            bufferData[3] = (bufferData[3] + transform.position.X) * transform.scale.X;
            bufferData[6] = (bufferData[6] + transform.position.X) * transform.scale.X;
            bufferData[9] = (bufferData[9] + transform.position.X) * transform.scale.X;

            bufferData[1] = (bufferData[1] + transform.position.Y) * transform.scale.Y;
            bufferData[4] = (bufferData[4] + transform.position.Y) * transform.scale.Y;
            bufferData[7] = (bufferData[7] + transform.position.Y) * transform.scale.Y;
            bufferData[10] = (bufferData[10] + transform.position.Y) * transform.scale.Y;

            UpdateBuffer(buffer, bufferData);
        }

        internal void RenderTexture(uint id)
        {
            if (!_shaders.ContainsKey(id) || !_vertexArray.ContainsKey(id) || !_selectedTexture.ContainsKey(id) || !_textures.ContainsKey(id)) return;

            Gl.UseProgram(_shaders[id]);
            Gl.Enable(EnableCap.Texture2d);
            Gl.BindVertexArray(_vertexArray[id]);
            Gl.BindTexture(TextureTarget.Texture2d, _textures[id][_selectedTexture[id]]);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.UseProgram(0);
            Gl.BindVertexArray(0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            Gl.Disable(EnableCap.Texture2d);
        }

        public void DrawQuad(bool fill, float x1, float y1, float x2, float y2, Color color = new Color())
        {
            PrimitiveType type = fill ? PrimitiveType.Quads : PrimitiveType.LineStrip;

            Gl.Color3(color.r, color.g, color.b);
            Matrix4x4f matrix = WindowController.Instance.Scaling;

            Gl.Begin(type);
            Gl.Vertex2((x1 * matrix.Row0.x) + (y1 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y1 * matrix.Row1.y));
            Gl.Vertex2((x1 * matrix.Row0.x) + (y2 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y2 * matrix.Row1.y));
            Gl.Vertex2((x2 * matrix.Row0.x) + (y2 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y2 * matrix.Row1.y));
            Gl.Vertex2((x2 * matrix.Row0.x) + (y1 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y1 * matrix.Row1.y));
            Gl.End();
        }

        public void DrawQuad(bool fill, Transform t, Color color = new Color())
        {
            PrimitiveType type = fill ? PrimitiveType.Quads : PrimitiveType.LineStrip;

            Gl.Color3(color.r, color.g, color.b);
            Matrix4x4f matrix = WindowController.Instance.Scaling;

            Gl.Begin(type);
            Gl.Vertex2(((t.position.X - t.scale.X) * matrix.Row0.x) + ((t.position.Y - t.scale.Y) * matrix.Row0.y), ((t.position.X - t.scale.X) * matrix.Row1.x) + ((t.position.Y - t.scale.Y) * matrix.Row1.y));
            Gl.Vertex2(((t.position.X - t.scale.X) * matrix.Row0.x) + ((t.position.Y + t.scale.Y) * matrix.Row0.y), ((t.position.X - t.scale.X) * matrix.Row1.x) + ((t.position.Y + t.scale.Y) * matrix.Row1.y));
            Gl.Vertex2(((t.position.X + t.scale.X) * matrix.Row0.x) + ((t.position.Y + t.scale.Y) * matrix.Row0.y), ((t.position.X + t.scale.X) * matrix.Row1.x) + ((t.position.Y + t.scale.Y) * matrix.Row1.y));
            Gl.Vertex2(((t.position.X + t.scale.X) * matrix.Row0.x) + ((t.position.Y - t.scale.Y) * matrix.Row0.y), ((t.position.X + t.scale.X) * matrix.Row1.x) + ((t.position.Y - t.scale.Y) * matrix.Row1.y));
            Gl.End();
        }

        internal void Render(List<uint> renderQueue) => renderQueue.ForEach(x => RenderTexture(x));

        internal void Dispose()
        {
            _buffers.Values.ToList().ForEach(x => Gl.DeleteBuffers(x.Select(y => y.id).ToArray()));
            _vertexArray.Values.ToList().ForEach(x => Gl.DeleteVertexArrays(x));
            _textures.Values.ToList().ForEach(x => x.ForEach(y => Gl.DeleteTextures(y)));
            _shaders.Values.ToList().ForEach(x => Gl.DeleteProgram(x));
            _buffers.Clear();
            _vertexArray.Clear();
            _textures.Clear();
            _shaders.Clear();
            _graphicsTransform.Clear();
        }

        internal void Dispose(uint id)
        {
            if (_buffers.ContainsKey(id)) _buffers[id].Select(x => x.id).ToList().ForEach(y => Gl.DeleteBuffers(y)); _buffers.Remove(id);
            if (_vertexArray.ContainsKey(id)) Gl.DeleteBuffers(_vertexArray[id]); _vertexArray.Remove(id);
            if (_textures.ContainsKey(id)) _textures[id].ForEach(y => Gl.DeleteBuffers(y)); _textures.Remove(id);
            if (_shaders.ContainsKey(id)) Gl.DeleteBuffers(_shaders[id]); _shaders.Remove(id);
            if (_graphicsTransform.ContainsKey(id)) _graphicsTransform.Remove(id);
        }
    }
}
