using System;
using OpenGL;
using System.Runtime.InteropServices;

namespace Lunar.GL
{
    public struct Buffer<T> where T : struct
    {
        public uint id;
        public string name;
        public int totalSize;
        public int size;
        public BufferTarget target;
        public T[] data;

        public Buffer(T[] data, int size, string attributeName, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsage bufferUsage = BufferUsage.DynamicDraw)
        {
            this.target = target;
            try { totalSize = Marshal.SizeOf(typeof(T)) * data.Length; }
            catch { Console.WriteLine("Buffer type was not a primitive"); totalSize = 0; this.size = 0; this.data = null; name = ""; id = 0; return; }

            this.data = data;
            this.size = size;
            name = attributeName;

            id = Gl.GenBuffer();

            Gl.BindBuffer(target, id);
            Gl.BufferData(target, (uint)totalSize, data, bufferUsage);
            Gl.BindBuffer(target, 0);
        }

        public Buffer(IntPtr data, int totalSize, int size, string attributeName, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsage bufferUsage = BufferUsage.DynamicDraw)
        {
            this.target = target;
            this.totalSize = totalSize;

            this.data = null;
            this.size = size;
            name = attributeName;

            id = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)totalSize, data, bufferUsage);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Copy<T>(IntPtr data, int length)
        {
            byte[] temp = new byte[length];
        }

        public void UpdateBuffer(T[] data)
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)totalSize, data);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateBuffer(IntPtr data)
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)totalSize, data);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(id);
        }
    }
}
