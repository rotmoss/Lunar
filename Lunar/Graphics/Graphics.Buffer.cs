using System.Collections.Generic;
using OpenGL;
using System.Linq;

namespace Lunar
{
    partial class Graphics
    {
        private Dictionary<uint, List<Buffer>> _buffer;

        public static float[] GenTextureSquare(int x, int y, int w, int h) => new float[] { x, y + h, x + w, y + h, x + w, y, x, y };
        public static float[] GenVertexSquare(int w, int h) => new float[] { -w, -h, 0, w, -h, 0, w, h, 0, -w, h, 0 };

        public struct Buffer
        {
            public uint id;
            public string name;
            public int size;
        }

        public Buffer CreateBuffer(float[] bufferData, string attributeName, int size)
        {
            uint buffer = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(4 * bufferData.Length), bufferData, BufferUsage.StreamDraw);

            return new Buffer { id = buffer, name = attributeName, size = size };
        }

        public void BindBuffer(uint id) => Gl.BindBuffer(BufferTarget.ArrayBuffer, id);
        public void UnBindBuffer() => Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        public void DeleteBuffers(uint id) => _buffer[id].ForEach(x => Gl.DeleteBuffers(x.id));
    }
}
