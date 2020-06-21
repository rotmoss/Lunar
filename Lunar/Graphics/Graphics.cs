using System;
using System.Collections.Generic;
using System.Linq;
using OpenGL;


namespace Lunar
{
    public partial class Graphics
    {
        private static Graphics instance = null;
        public static Graphics Instance { get { instance = instance == null ? new Graphics() : instance; return instance; } }

        private Dictionary<uint, Matrix4x4f> _matrix;

        private Graphics()
        {
            _shader = new Dictionary<uint, uint>();
            _texture = new Dictionary<uint, List<uint>>();
            _selectedTexture = new Dictionary<uint, int>();
            _vertexArray = new Dictionary<uint, uint>();
            _buffer = new Dictionary<uint, List<Buffer>>();
            _matrix = new Dictionary<uint, Matrix4x4f>();
        }

        public void CreateSprite(uint id, string texture, string vs, string fs)
        {
            if (!_matrix.ContainsKey(id)) { _matrix.Add(id, new Matrix4x4f()); }
            _matrix[id] = Matrix4x4f.Identity;

            if (_shader.ContainsKey(id)) { Gl.DeleteProgram(_shader[id]); _shader.Remove(id); }
            _shader.Add(id, CreateShader(vs, fs));

            if (!_texture.ContainsKey(id)) { _texture.Add(id, new List<uint>()); }
            _texture[id].Add(CreateTexture(texture, out int w, out int h));

            if(!_selectedTexture.ContainsKey(id)) { _selectedTexture.Add(id, 0); }

            if (_buffer.ContainsKey(id)) { _buffer[id].ForEach(x => Gl.DeleteProgram(x.id)); _buffer.Remove(id); }
            _buffer.Add(id, new List<Buffer>());

            _buffer[id].Add(CreateBuffer(GenVertexSquare(w, h), "aPos", 3));
            _buffer[id].Add(CreateBuffer(GenTextureSquare(0, 0, 1, 1), "aTexCoord", 2));

            if (_vertexArray.ContainsKey(id)) { Gl.DeleteVertexArrays(_vertexArray[id]); _vertexArray.Remove(id); }
            _vertexArray.Add(id, CreateVertexArray(_shader[id], _buffer[id]));
        }

        public void ScaleMatrix(uint id, float w, float h)
        {
            if (_matrix.ContainsKey(id)) 
            {
                Matrix4x4f temp = _matrix[id];
                temp.Scale(w, h, 1);
                _matrix[id] = temp;
            }
        }

        public void UpdateMatrix(Dictionary<uint, Transform> transforms)
        {
            foreach (KeyValuePair<uint, Transform> pair in transforms)
            {
                if (_matrix.ContainsKey(pair.Key))
                    SetUniform(pair.Key, _matrix[pair.Key] * pair.Value.ToMatrix4x4f(), "uModelView");
                else
                    SetUniform(pair.Key, pair.Value.ToMatrix4x4f(), "uModelView");
            }
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
            _buffer.Clear();
            _vertexArray.Values.ToList().ForEach(x => Gl.DeleteVertexArrays(x));
            _vertexArray.Clear();
            _shader.Values.ToList().ForEach(x => Gl.DeleteProgram(x));
            _texture.Clear();
            _texture.Values.ToList().ForEach(x => x.ForEach(y => Gl.DeleteTextures(y)));
            _shader.Clear();
        }
    }
}
