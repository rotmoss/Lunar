using System;
using System.Collections.Generic;
using OpenGL;

namespace Lunar.Graphics
{
    public class VertexArray
    {
        internal ShaderProgram shaderProgram;
        public uint id;

        public VertexArray()
        {
            id = 0;
        }

        public static bool CreateVertexArray(ShaderProgram shaderProgram, out VertexArray vertexArray)
        {
            if (shaderProgram == null) { vertexArray = null; return false; }

            vertexArray = new VertexArray();
            vertexArray.id = Gl.GenVertexArray();
            vertexArray.shaderProgram = shaderProgram;

            return true;
        }

        public bool AddBuffer<T>(params Buffer<T>[] buffers) where T : struct
        {
            Gl.BindVertexArray(id);

            foreach (Buffer<T> buffer in buffers)
            {
                if (buffer.id == 0) return false;
                Gl.BindBuffer(buffer.target, buffer.id);
                uint attributeLocation = (uint)Gl.GetAttribLocation(shaderProgram.id, buffer.name);

                Gl.VertexAttribPointer(attributeLocation, buffer.size, GetAttribType<T>(), false, 0, IntPtr.Zero);
                Gl.EnableVertexAttribArray(attributeLocation);
            }

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return true;
        }

        public VertexAttribType GetAttribType<T>()
        {
            if (typeof(T) == typeof(float)) return VertexAttribType.Float;
            if (typeof(T) == typeof(double)) return VertexAttribType.Double;
            if (typeof(T) == typeof(int)) return VertexAttribType.Int;
            if (typeof(T) == typeof(byte)) return VertexAttribType.Byte;
            if (typeof(T) == typeof(uint)) return VertexAttribType.UnsignedInt;
            else return VertexAttribType.Fixed;
        }

        public void Dispose()
        {
            Gl.DeleteVertexArrays(id);
        }
    }
}
