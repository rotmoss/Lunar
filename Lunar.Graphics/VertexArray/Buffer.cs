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
        public double[] data;

        public Buffer(double[] data, int size, string attributeName)
        {
            this.data = data;
            this.size = size;
            name = attributeName;

            float[] floatData = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
                floatData[i] = (float)data[i];

            id = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(sizeof(float) * data.Length), floatData, BufferUsage.StaticCopy);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateBuffer(double[] data)
        {
            float[] floatData = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
                floatData[i] = (float)System.Math.Floor(data[i] * 0.5) / 0.5f;

            Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)(sizeof(float) * data.Length), floatData);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(id);
        }
    }
}
