using System;
using System.Collections.Generic;
using OpenGL;

namespace Lunar.Graphics
{
    public class VertexArray
    {
        internal uint id;

        public VertexArray()
        {
            id = 0;
        }

        internal static bool CreateVertexArray(ShaderProgram shaderProgram, out VertexArray vertexArray, params BufferObject[] buffers)
        {
            if (shaderProgram == null) { vertexArray = null; return false; }

            vertexArray = new VertexArray();

            vertexArray.id = Gl.GenVertexArray();

            Gl.BindVertexArray(vertexArray.id);

            foreach (BufferObject buffer in buffers)
            {
                if (buffer.id == 0) return false;
                Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer.id);
                uint attributeLocation = (uint)Gl.GetAttribLocation(shaderProgram.id, buffer.name);

                Gl.VertexAttribPointer(attributeLocation, buffer.size, VertexAttribType.Float, false, 0, IntPtr.Zero);
                Gl.EnableVertexAttribArray(attributeLocation);
            }

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return true;
        }

        public void Dispose()
        {
            Gl.DeleteVertexArrays(id);
        }
    }
}
