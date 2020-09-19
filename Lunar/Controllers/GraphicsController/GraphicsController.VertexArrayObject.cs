using OpenGL;
using System;
using System.Collections.Generic;

namespace Lunar
{
    partial class GraphicsController
    {
        internal uint CreateVertexArray(uint shaderProgram, params BufferObject[] buffers)
        {
            uint vertexArray = Gl.GenVertexArray();

            Gl.BindVertexArray(vertexArray);

            foreach (BufferObject buffer in buffers)
            {
                Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer.id);
                uint attributeLocation = (uint)Gl.GetAttribLocation(shaderProgram, buffer.name);

                Gl.VertexAttribPointer(attributeLocation, buffer.size, VertexAttribType.Float, false, 0, IntPtr.Zero);
                Gl.EnableVertexAttribArray(attributeLocation);
            }

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vertexArray;
        }
    }
}
