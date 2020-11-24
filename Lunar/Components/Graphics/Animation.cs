using Lunar.Graphics;
using OpenGL;

namespace Lunar
{
    public class Animation : GraphicsComponent
    {
        enum ScrollDirection { Vertical, Horizontal }

        public Texture[] Textures { get => _textures; }
        private Texture[] _textures;

        public Buffer<float> TexCoordsBuffer { get => _texCoordsBuffer; }
        private Buffer<float> _texCoordsBuffer;

        public int Width { get => _width; }
        private int _width;

        public int Height { get => _height; }
        private int _height;

        private float _frameWidth;
        private float _frameHeight;

        private double _timeSinceLastFrame;
        private double _timePerFrame;

        public Animation(string vs, string fs, double frameRate, int frameWidth, int frameHeight, params string[] textureFiles) : base(vs, fs)
        {
            int w = 0, h = 0;

            _textures = new Texture[textureFiles.Length];

            for (int i = 0; i < textureFiles.Length; i++)
                if (!Texture.CreateTextureFromFile(textureFiles[i], out w, out h, out _textures[i])) { Dispose(); return; }

            _frameWidth = frameWidth / (float)w;
            _frameHeight = frameHeight / (float)h;

            _positionBuffer = new Buffer<float>(new float[] { -frameWidth, -frameHeight, frameWidth, -frameHeight, frameWidth, frameHeight, -frameWidth, frameHeight }, 2, "aPos");
            _texCoordsBuffer = new Buffer<float>(new float[] { 0, _frameHeight, _frameWidth, _frameHeight, _frameWidth, 0, 0, 0 }, 2, "aTexCoord");

            _vertexArray.AddBuffer(_positionBuffer, _texCoordsBuffer);

            _timePerFrame = 1.0d / frameRate;

            _width = frameWidth; _height = frameHeight;
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
            for (int i = 0; i < _textures.Length; i++) _textures[i]?.Dispose();
            _vertexArray?.Dispose();
            _positionBuffer.Dispose();
            _texCoordsBuffer.Dispose();
        }

        public void NextFrame(double frameTime)
        {
            if (_timeSinceLastFrame < _timePerFrame) { _timeSinceLastFrame += frameTime; return; }
            else { _timeSinceLastFrame = 0; }

            float[] data = _texCoordsBuffer.data;

            if (data[0] + _frameWidth >= 1) ResetTextureCoords(ref data, ScrollDirection.Horizontal);
            else ScrollTextureCoords(ref data, ScrollDirection.Horizontal);

            if (data[1] + _frameHeight >= 1) ResetTextureCoords(ref data, ScrollDirection.Vertical);
            else ScrollTextureCoords(ref data, ScrollDirection.Vertical);

            _texCoordsBuffer.UpdateBuffer(data);
        }

        private void ScrollTextureCoords(ref float[] data, ScrollDirection scrollDirection)
        {
            for (int i = scrollDirection == ScrollDirection.Horizontal ? 0 : 1; i < data.Length; i += 2)
                data[i] += scrollDirection == ScrollDirection.Horizontal ? _frameWidth : _frameHeight;
        }

        private void ResetTextureCoords(ref float[] data, ScrollDirection scrollDirection)
        {
            data = scrollDirection == ScrollDirection.Horizontal ? 
                new float[] { 0, data[1], _frameWidth, data[3], _frameWidth, data[5], 0, data[7] } :
                new float[] { data[0], _frameHeight, data[3], _frameHeight, data[4], 0, data[6], 0 } ;
        }

        public static void Animate(double frameTime)
        {
            foreach(GraphicsComponent component in _components)
                if(component.GetType() == typeof(Animation)) ((Animation)component).NextFrame(frameTime);            
        }

        public static new Animation GetComponent(uint id)
        {
            for (int i = 0; i < _dictionary[id].Count; i++)
                if (_dictionary[id][i].GetType() == typeof(Animation)) return (Animation)_dictionary[id][i];
            return default;
        }
    }
}
