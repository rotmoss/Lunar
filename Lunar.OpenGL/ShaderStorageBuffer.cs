using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Lunar.OpenGL
{
    public interface IShaderStorageBuffer : IDisposable
    {
        public object Data { get; set; }
        public void Update();
    }
    
    public struct ShaderStorageBuffer<T> : IShaderStorageBuffer where T : unmanaged
    {
        public uint id;
        public object Data { get => _data; set { _data = (T)value; Update(); } }
        private T _data;
        private uint _size;

        public ShaderStorageBuffer(uint location, T data)
        {
            _data = data;
            _size = (uint)Marshal.SizeOf(_data);
            id = Engine.GL.GenBuffer();

            Engine.GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, id);

            Engine.GL.BufferData(BufferTargetARB.ShaderStorageBuffer, _size, _data, BufferUsageARB.DynamicCopy);
            Engine.GL.BindBufferBase(BufferTargetARB.ShaderStorageBuffer, location, id);

            Engine.GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, 0);
        }

        public ShaderStorageBuffer(uint location, uint size, T data)
        {
            _data = data;
            _size = size;
            id = Engine.GL.GenBuffer();

            Engine.GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, id);

            Engine.GL.BufferData(BufferTargetARB.ShaderStorageBuffer, size, _data, BufferUsageARB.DynamicCopy);
            Engine.GL.BindBufferBase(BufferTargetARB.ShaderStorageBuffer, location, id);

            Engine.GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, 0);
        }

        public void Update()
        {
            Engine.GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, id);
            Engine.GL.BufferSubData(BufferTargetARB.ShaderStorageBuffer, IntPtr.Zero, _size, _data);

            Engine.GL.BindBuffer(BufferTargetARB.ShaderStorageBuffer, 0);
        }

        public void Dispose() 
        {
            Engine.GL.DeleteBuffer(id);
        }
    }
}