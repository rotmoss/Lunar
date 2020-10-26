using System;
using OpenGL;
using System.Runtime.InteropServices;

namespace Lunar.Graphics
{
    public struct Buffer
    {
        public uint id;
        public string name;
        public int size;
        public float[] data;

        public Buffer(float[] data, int size, string attributeName)
        {
            this.data = data;
            this.size = size;
            name = attributeName;

            id = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(sizeof(float) * data.Length), data, BufferUsage.StaticCopy);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateBuffer(float[] data)
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)(sizeof(float) * data.Length), data);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(id);
        }
    }
}
