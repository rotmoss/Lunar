using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Lunar
{
    partial class Graphics
    {
        private Dictionary<uint, uint> _vertexArray;

        public uint CreateVertexArray(uint shaderProgram, List<Buffer> buffers)
        {
            uint vertexArray = Gl.GenVertexArray();

            Gl.BindVertexArray(vertexArray);

            foreach (Buffer buffer in buffers)
            {
                Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer.id);
                Gl.VertexAttribPointer((uint)Gl.GetAttribLocation(shaderProgram, buffer.name), buffer.size, VertexAttribType.Float, false, 0, IntPtr.Zero);
                Gl.EnableVertexAttribArray((uint)Gl.GetAttribLocation(shaderProgram, buffer.name));
            }

            Gl.BindVertexArray(0);

            return vertexArray;
        }

        public void BindVertexArray(uint id) => Gl.BindVertexArray(_vertexArray[id]);
        public void UnBindVertexArray() => Gl.BindVertexArray(0);
        public void DeleteVertexArray(uint id) => Gl.DeleteVertexArrays(_vertexArray[id]);
    }
}
