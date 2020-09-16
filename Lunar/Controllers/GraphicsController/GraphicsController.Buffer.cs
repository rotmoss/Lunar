using OpenGL;
using System;
using System.Collections.Generic;

namespace Lunar
{
    partial class GraphicsController
    {
        private Dictionary<uint, List<Buffer>> _buffers;

        public static float[] GenTextureSquare(int x, int y, int w, int h) => new float[] { x, y + h, x + w, y + h, x + w, y, x, y };
        public static float[] GenVertexSquare(int w, int h) => new float[] { -w, -h, 0, w, -h, 0, w, h, 0, -w, h, 0 };

        internal struct Buffer
        {
            public uint id;
            public string name;
            public int size;
            public float[] data;
        }

        internal Buffer CreateBuffer(float[] bufferData, string attributeName, int size)
        {
            uint buffer = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(4 * bufferData.Length), bufferData, BufferUsage.StaticCopy);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return new Buffer { id = buffer, name = attributeName, size = size, data = bufferData };
        }

        internal void UpdateBuffer(Buffer buffer, float[] bufferData)
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer.id);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)(4 * bufferData.Length), bufferData);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        internal void BindBuffer(uint id) => Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
        internal void UnBindBuffer() => Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        internal void DeleteBuffers(uint id) => _buffers[id].ForEach(x => Gl.DeleteBuffers(x.id));
    }
}
