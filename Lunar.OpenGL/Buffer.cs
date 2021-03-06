using System;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace Lunar.OpenGL
{
    public struct Buffer<T> where T : unmanaged
    {
        public uint id;
        public int totalSize;
        public BufferTargetARB target;

        public unsafe Buffer(ReadOnlySpan<T> data,  BufferTargetARB target = BufferTargetARB.ArrayBuffer, BufferUsageARB bufferUsage = BufferUsageARB.DynamicDraw)
        {
            this.target = target;
            totalSize = Marshal.SizeOf(typeof(T)) * data.Length;

            id = Engine.GL.GenBuffer();

            Engine.GL.BindBuffer(target, id);
            Engine.GL.BufferData(target, (uint)totalSize * 100, data, bufferUsage);
            Engine.GL.BindBuffer(target, 0);
        }

        public void UpdateBuffer(ReadOnlySpan<T> data)
        {
            Engine.GL.BindBuffer(BufferTargetARB.ArrayBuffer, id);
            Engine.GL.BufferSubData(BufferTargetARB.ArrayBuffer, IntPtr.Zero, (uint)totalSize, data);

            Engine.GL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            Engine.GL.DeleteBuffer(id);
        }
    }
}