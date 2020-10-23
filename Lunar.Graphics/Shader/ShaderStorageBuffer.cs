using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenGL;

namespace Lunar.Graphics
{
    public struct ShaderStorageBuffer<T>
    {
        public uint id;
        public T Data { get => _data; set { _data = value; Update(); } }
        private T _data;
        private uint _size;

        public ShaderStorageBuffer(uint location, T data)
        {
            _data = data;
            _size = (uint)Marshal.SizeOf(_data);
            id = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ShaderStorageBuffer, id);

            Gl.BufferData(BufferTarget.ShaderStorageBuffer, _size, _data, BufferUsage.DynamicCopy);
            Gl.BindBufferBase(BufferTarget.ShaderStorageBuffer, location, id);

            Gl.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        public ShaderStorageBuffer(uint location, uint size, T data)
        {
            _data = data;
            _size = size;
            id = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ShaderStorageBuffer, id);

            Gl.BufferData(BufferTarget.ShaderStorageBuffer, size, _data, BufferUsage.DynamicCopy);
            Gl.BindBufferBase(BufferTarget.ShaderStorageBuffer, location, id);

            Gl.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        public void Update()
        {
            Gl.BindBuffer(BufferTarget.ShaderStorageBuffer, id);
            Gl.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, _size, _data);

            Gl.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }
    }
}