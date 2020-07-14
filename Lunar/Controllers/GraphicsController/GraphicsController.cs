﻿using System;
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
            _shaders = new Dictionary<uint, uint>();
            _texture = new Dictionary<uint, List<uint>>();
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
            if (!_texture.ContainsKey(id)) { _texture.Add(id, new List<uint>()); }
            _texture[id].Add(CreateTexture(texture, out w, out h));
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
            AddShader(id, null, null);
            AddTexture(id, file, out w, out h);
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

        internal void RenderQuad(uint id)
        {
            Gl.UseProgram(_shaders[id]);
            Gl.Enable(EnableCap.Texture2d);
            Gl.BindVertexArray(_vertexArray[id]);
            Gl.BindTexture(TextureTarget.Texture2d, _texture[id][_selectedTexture[id]]);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.UseProgram(0);
            Gl.BindVertexArray(0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            Gl.Disable(EnableCap.Texture2d);
        }

        internal void Render(List<uint> renderQueue) => renderQueue.ForEach(x => RenderQuad(x));

        internal void Dispose()
        {
            _buffers.Values.ToList().ForEach(x => Gl.DeleteBuffers(x.Select(y => y.id).ToArray()));
            _vertexArray.Values.ToList().ForEach(x => Gl.DeleteVertexArrays(x));
            _texture.Values.ToList().ForEach(x => x.ForEach(y => Gl.DeleteTextures(y)));
            _shaders.Values.ToList().ForEach(x => Gl.DeleteProgram(x));
            _buffers.Clear();
            _vertexArray.Clear();
            _texture.Clear();
            _shaders.Clear();
            _graphicsTransform.Clear();
        }

        internal void Dispose(uint id)
        {
            if (_buffers.ContainsKey(id)) _buffers[id].Select(x => x.id).ToList().ForEach(y => Gl.DeleteBuffers(y)); _buffers.Remove(id);
            if (_vertexArray.ContainsKey(id)) Gl.DeleteBuffers(_vertexArray[id]); _vertexArray.Remove(id);
            if (_texture.ContainsKey(id)) _texture[id].ForEach(y => Gl.DeleteBuffers(y)); _texture.Remove(id);
            if (_shaders.ContainsKey(id)) Gl.DeleteBuffers(_shaders[id]); _shaders.Remove(id);
            if (_graphicsTransform.ContainsKey(id)) _graphicsTransform.Remove(id);
        }
    }
}
