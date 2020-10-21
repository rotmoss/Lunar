using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Lunar.Graphics
{
    public class Shape : RenderData
    {
        bool _wireFrame;
        public Shape(uint Id, string vertexShader, string fragmentShader, int vertexSize, bool wireFrame, params float[] vertecies)
        {
            id = Id;
            Visible = true;
            _wireFrame = wireFrame;

            positionBuffer = new BufferObject(vertecies, vertexSize, "aPos");

            if (!ShaderProgram.CreateShader(vertexShader, fragmentShader, out shaderProgram)) { Dispose(); return; }
            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray, positionBuffer)) { Dispose(); return; }

            _renderData.Add(this);
        }

        public Shape(uint Id, string vsName, string[] vertexShader, string fsName, string[] fragmentShader, int vertexSize, bool wireFrame, params float[] vertecies)
        {
            id = Id;
            Visible = true;
            _wireFrame = wireFrame;

            positionBuffer = new BufferObject(vertecies, vertexSize, "aPos");

            if (!ShaderProgram.CreateShader(vsName, vertexShader, fsName, fragmentShader, out shaderProgram)) { Dispose(); return; }
            if (!VertexArray.CreateVertexArray(shaderProgram, out vertexArray, positionBuffer)) { Dispose(); return; }

            _renderData.Add(this);
        }  

        public override void Dispose()
        {
            vertexArray?.Dispose();
            positionBuffer.Dispose();
            _renderData.Remove(this);
        }

        public override void Render()
        {
            if (!Visible || shaderProgram == null || vertexArray == null) return;

            if (_wireFrame) Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            Gl.UseProgram(shaderProgram.id);
            Gl.BindVertexArray(vertexArray.id);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}
