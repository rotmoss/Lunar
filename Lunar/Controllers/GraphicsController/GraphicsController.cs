using OpenGL;
using System.Collections.Generic;
using System.Linq;

namespace Lunar
{
    public partial class GraphicsController
    {
        private static GraphicsController instance = null;
        public static GraphicsController Instance { get { instance = instance == null ? new GraphicsController() : instance; return instance; } }
        private GraphicsController()
        {
            _shaders = new Dictionary<uint, uint>();
            _uniforms = new Dictionary<uint, Dictionary<string, int>>();
            _textures = new Dictionary<uint, List<uint>>();
            _selectedTexture = new Dictionary<uint, int>();
            _vertexArray = new Dictionary<uint, uint>();
            _buffers = new Dictionary<uint, List<Buffer>>();
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
        }
    
        public void CreateText(uint id, string font, string text, int size, Color color, uint wrapper, string vertexShader, string fragmentShader, out int w, out int h)
        {
            AddShader(id, vertexShader, fragmentShader);
            AddText(id, font, text, size, color, wrapper, out w, out h);
            AddBuffer(id, w, h);
        }

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
        }

        public void Dispose(uint id)
        {
            if (_buffers.ContainsKey(id)) _buffers[id].Select(x => x.id).ToList().ForEach(y => Gl.DeleteBuffers(y)); _buffers.Remove(id);
            if (_vertexArray.ContainsKey(id)) Gl.DeleteBuffers(_vertexArray[id]); _vertexArray.Remove(id);
            if (_textures.ContainsKey(id)) _textures[id].ForEach(y => Gl.DeleteBuffers(y)); _textures.Remove(id);
            if (_shaders.ContainsKey(id)) Gl.DeleteBuffers(_shaders[id]); _shaders.Remove(id);
        }
    }
}
