using Lunar.GL;
using OpenGL;

namespace Lunar.ECS.Components
{
    public class Text : GraphicsComponent
    {
        public Texture Texture { get => _texture; }
        private Texture _texture;

        public Buffer<float> TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer<float> _texCoordsBuffer;

        public int Width { get => _width; }
        private int _width;

        public int Height { get => _height; }
        private int _height;

        public Text(string vs, string fs, string fontFile, string message, int size, uint wrapped, byte r, byte g, byte b, byte a) : base(vs, fs)
        {
            if (!Texture.CreateTextureFromText(fontFile, message, size, wrapped, r, g, b, a, out int w, out int h, out _texture)){ Dispose(); return; }

            _positionBuffer = new Buffer<float>(new float[] { -w, -h, w, -h, w, h, -w, h }, 2, "aPos");
            _texCoordsBuffer = new Buffer<float>(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 }, 2, "aTexCoord");

            _width = w;
            _height = h;

            _vertexArray.AddBuffer(_positionBuffer, _texCoordsBuffer);
        }

        public void UpdateText(string fontFile, string message, int size, uint wrapped, byte r, byte g, byte b, byte a)
        {
            _texture.Dispose();
            Texture.CreateTextureFromText(fontFile, message, size, wrapped, r, g, b, a, out int w, out int h, out _texture);

            _texCoordsBuffer.UpdateBuffer(new float[] { -w, -h, w, -h, w, h, -w, h });

            _width = w;
            _height = h;
        }

        public override void Render()
        {
            Gl.Enable(EnableCap.Texture2d);

            if (!_enabled || _shaderProgram == null || _vertexArray == null || _texture == null) return;

            Gl.UseProgram(_shaderProgram.id);
            Gl.BindVertexArray(_vertexArray.id);
            Gl.BindTexture(TextureTarget.Texture2d, _texture.id);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.Disable(EnableCap.Texture2d);
        }

        public override void DisposeChild()
        {
            _vertexArray?.Dispose();
            _texture?.Dispose();
            _positionBuffer.Dispose();
            _texCoordsBuffer.Dispose();
        }

        public static new Text GetComponent(uint id)
        {
            for (int i = 0; i < _entryByOwner[id].Count; i++)
                if (_entryByOwner[id][i].GetType() == typeof(Text)) return (Text)_entryByOwner[id][i];
            return default;
        }
    }
}
