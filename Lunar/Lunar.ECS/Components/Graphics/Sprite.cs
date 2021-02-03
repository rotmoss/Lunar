using Lunar.GL;
using OpenGL;

namespace Lunar.ECS.Components
{
    public class Sprite : GraphicsComponent
    {
        public Texture[] Textures { get => _textures; }
        private Texture[] _textures;

        public Buffer<float> TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer<float> _texCoordsBuffer;

        public int Width { get => _width; }
        private int _width;

        public int Height { get => _height; }
        private int _height;

        public Sprite(string vs, string fs, params string[] textureFiles) : base(vs, fs)
        {
            _textures = new Texture[textureFiles.Length];

            int w = 0, h = 0;
            for (int i = 0; i < textureFiles.Length; i++)
                if (!Texture.CreateTextureFromFile(textureFiles[i], out w, out h, out _textures[i])) { Dispose(); return; }

            _positionBuffer = new Buffer<float>(new float[] { -w, -h, w, -h, w, h, -w, h }, 2, "aPos");
            _texCoordsBuffer = new Buffer<float>(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord");

            _width = w;
            _height = h;

            _vertexArray.AddBuffer(_positionBuffer, _texCoordsBuffer);
        }

        public override void Render()
        {
            Gl.Enable(EnableCap.Texture2d);

            if (!_enabled || _shaderProgram == null || _vertexArray == null || _textures == null) return;

            Gl.UseProgram(_shaderProgram.id);
            Gl.BindVertexArray(_vertexArray.id);

            for (int i = 0; i < _textures.Length; i++) {
                Gl.ActiveTexture(TextureUnit.Texture0 + i);
                Gl.BindTexture(TextureTarget.Texture2d, _textures[i].id);
            }

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.Disable(EnableCap.Texture2d);
        }

        public override void DisposeChild()
        {
            for(int i = 0; i < _textures.Length; i++) 
                _textures[i]?.Dispose();

            _vertexArray?.Dispose();
            _positionBuffer.Dispose();
            _texCoordsBuffer.Dispose();
        }

        public static new Sprite GetComponent(uint id)
        {
            for (int i = 0; i < _entryByOwner[id].Count; i++)
                if (_entryByOwner[id][i].GetType() == typeof(Sprite)) return (Sprite)_entryByOwner[id][i];
            return default;
        }
    }
}
