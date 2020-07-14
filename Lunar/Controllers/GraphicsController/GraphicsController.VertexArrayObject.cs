using System;
using System.Collections.Generic;
using OpenGL;

namespace Lunar
{
    partial class GraphicsController
    {
        private Dictionary<uint, uint> _vertexArray;

        internal uint CreateVertexArray(uint shaderProgram, List<Buffer> buffers)
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
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vertexArray;
        }

        internal void BindVertexArray(uint id) => Gl.BindVertexArray(_vertexArray[id]);
        internal void UnBindVertexArray() => Gl.BindVertexArray(0);
        internal void DeleteVertexArray(uint id) => Gl.DeleteVertexArrays(_vertexArray[id]);
    }
}
