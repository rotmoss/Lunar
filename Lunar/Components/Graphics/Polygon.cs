using Lunar.Graphics;
using OpenGL;

namespace Lunar
{
    public class Polygon : GraphicsComponent
    {
        bool _wireFrame;
        public Polygon(string vs, string fs, int vertexSize, bool wireFrame, params float[] vertecies) : base(vs, fs)
        {
            _wireFrame = wireFrame;

            _positionBuffer = new Buffer<float>(vertecies, vertexSize, "aPos");

            if (!ShaderProgram.CreateShader(vs, fs, out _shaderProgram)) { Dispose(); return; }
            if (!VertexArray.CreateVertexArray(_shaderProgram, out _vertexArray)) { Dispose(); return; }
            _vertexArray.AddBuffer(_positionBuffer);
        }

        public override void Render()
        {
            if (!_enabled || _shaderProgram == null || _vertexArray == null) return;

            if (_wireFrame) Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            Gl.UseProgram(_shaderProgram.id);
            Gl.BindVertexArray(_vertexArray.id);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public override void DisposeChild()
        {
            _vertexArray?.Dispose();
            _positionBuffer.Dispose();
        }
    }
}
