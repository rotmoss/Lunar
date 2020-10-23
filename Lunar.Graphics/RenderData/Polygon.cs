using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Lunar.Graphics
{
    public class Polygon : RenderData
    {
        bool _wireFrame;
        public Polygon(uint Id, string vertexShader, string fragmentShader, int vertexSize, bool wireFrame, params double[] vertecies)
        {
            id = Id;
            Visible = true;
            _wireFrame = wireFrame;

            _positionBuffer = new Buffer(vertecies, vertexSize, "aPos");

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out _shaderProgram)) { Dispose(); return; }
            if (!VertexArray.CreateVertexArray(_shaderProgram, out _vertexArray, _positionBuffer)) { Dispose(); return; }

            Window.AddRenderData(this);
        }

        public override void Render()
        {
            if (!Visible || _shaderProgram == null || _vertexArray == null) return;

            if (_wireFrame) Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            Gl.UseProgram(_shaderProgram.id);
            Gl.BindVertexArray(_vertexArray.id);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public override void Dispose()
        {
            _vertexArray?.Dispose();
            _positionBuffer.Dispose();
            Window.RemoveRenderData(this);
        }
    }
}
