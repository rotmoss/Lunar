using System;
using OpenGL;

namespace Lunar.Graphics
{
    public struct BufferObject
    {
        public uint id;
        public string name;
        public int size;
        public float[] data;

        public BufferObject(float[] data, int size, string attributeName)
        {
            this.data = data;
            this.size = size;
            name = attributeName;

            id = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(4 * data.Length), data, BufferUsage.StaticCopy);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateBuffer(float[] data)
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)(4 * data.Length), data);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(id);
        }
    }
}
