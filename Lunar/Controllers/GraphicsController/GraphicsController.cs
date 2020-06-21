using System;
using System.Collections.Generic;
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
            _shader = new Dictionary<uint, uint>();
            _texture = new Dictionary<uint, List<uint>>();
            _selectedTexture = new Dictionary<uint, int>();
            _vertexArray = new Dictionary<uint, uint>();
            _buffer = new Dictionary<uint, List<Buffer>>();
            _graphicsTransform = new Dictionary<uint, Transform>();
        }

        public void AddShader(uint id, string vs, string fs)
        {
            if (_shader.ContainsKey(id)) { Gl.DeleteProgram(_shader[id]); _shader.Remove(id); }
            _shader.Add(id, CreateShader(vs, fs));
        }

        public void AddTexture(uint id, string texture, out int w, out int h)
        {
            if (!_texture.ContainsKey(id)) { _texture.Add(id, new List<uint>()); }
            _texture[id].Add(CreateTexture(texture, out w, out h));
            if (!_selectedTexture.ContainsKey(id)) { _selectedTexture.Add(id, 0); }
        }

        public void AddBuffer(uint id, int w, int h, int textureX = 0, int textureY = 0, int textureW = 1, int textureH = 1)
        {
            if (_buffer.ContainsKey(id)) { _buffer[id].ForEach(x => Gl.DeleteProgram(x.id)); _buffer.Remove(id); }
            _buffer.Add(id, new List<Buffer>());

            _buffer[id].Add(CreateBuffer(GenVertexSquare(w, h), "aPos", 3));
            _buffer[id].Add(CreateBuffer(GenTextureSquare(textureX, textureY, textureW, textureH), "aTexCoord", 2));

            if (_vertexArray.ContainsKey(id)) { Gl.DeleteVertexArrays(_vertexArray[id]); _vertexArray.Remove(id); }
            _vertexArray.Add(id, CreateVertexArray(_shader[id], _buffer[id]));
        }

        /// <summary> Uses the graphics transform aswell as the entity transform to translate the vertexbuffer linked to the spcified id. </summary>
        /// <param name="transforms"> A dictionary containing all entity ids and their respective entity transform </param>
        public void TranslateBuffers(Dictionary<uint, Transform> transforms)
        {
            foreach (KeyValuePair<uint, Transform> pair in transforms)
            {
                if (_graphicsTransform.ContainsKey(pair.Key) && _buffer.ContainsKey(pair.Key)) {
                    TranslateBuffer(pair.Key, pair.Value + _graphicsTransform[pair.Key]);
                }               
            }
        }

        private void TranslateBuffer(uint id, Transform transform)
        {
            Buffer buffer = _buffer[id].Where(x => x.name == "aPos").FirstOrDefault();

            float[] bufferData = new float[buffer.data.Length];
            for (int i = 0; i < bufferData.Length; i++) bufferData[i] = buffer.data[i];

            bufferData[0] = (bufferData[0] + transform.x) * transform.w;
            bufferData[3] = (bufferData[3] + transform.x) * transform.w;
            bufferData[6] = (bufferData[6] + transform.x) * transform.w;
            bufferData[9] = (bufferData[9] + transform.x) * transform.w;

            bufferData[1] = (bufferData[1] + transform.y) * transform.h;
            bufferData[4] = (bufferData[4] + transform.y) * transform.h;
            bufferData[7] = (bufferData[7] + transform.y) * transform.h;
            bufferData[10] = (bufferData[10] + transform.y) * transform.h;

            UpdateBuffer(buffer, bufferData);
        }

        public void RenderQuad(uint id)
        {
            Gl.UseProgram(_shader[id]);
            Gl.Enable(EnableCap.Texture2d);
            Gl.BindVertexArray(_vertexArray[id]);
            Gl.BindTextures(_texture[id][_selectedTexture[id]]);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.UseProgram(0);
            Gl.BindVertexArray(0);
            Gl.Disable(EnableCap.Texture2d);
            Gl.BindTextures(0);
        }

        public void Render(List<uint> renderQueue) => renderQueue.ForEach(x => RenderQuad(x));

        public void Dispose()
        {
            _buffer.Values.ToList().ForEach(x => Gl.DeleteBuffers(x.Select(y => y.id).ToArray()));
            _vertexArray.Values.ToList().ForEach(x => Gl.DeleteVertexArrays(x));
            _texture.Values.ToList().ForEach(x => x.ForEach(y => Gl.DeleteTextures(y)));
            _shader.Values.ToList().ForEach(x => Gl.DeleteProgram(x));
            _buffer.Clear();
            _vertexArray.Clear();
            _texture.Clear();
            _shader.Clear();
            _graphicsTransform.Clear();
        }

        public void Dispose(uint id)
        {
            if (_buffer.ContainsKey(id)) _buffer[id].Select(x => x.id).ToList().ForEach(y => Gl.DeleteBuffers(y)); _buffer.Remove(id);
            if (_vertexArray.ContainsKey(id)) Gl.DeleteBuffers(_vertexArray[id]); _vertexArray.Remove(id);
            if (_texture.ContainsKey(id)) _texture[id].ForEach(y => Gl.DeleteBuffers(y)); _texture.Remove(id);
            if (_shader.ContainsKey(id)) Gl.DeleteBuffers(_shader[id]); _shader.Remove(id);
            if (_graphicsTransform.ContainsKey(id)) _graphicsTransform.Remove(id);
        }
    }
}
