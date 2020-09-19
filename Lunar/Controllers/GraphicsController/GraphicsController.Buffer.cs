using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Lunar
{
    public struct BufferObject
    {
        public uint id;
        public string name;
        public int size;
        public float[] data;
    }

    partial class GraphicsController
    {
        public static float[] GenTextureSquare(int x, int y, int w, int h) => new float[] { x, y + h, x + w, y + h, x + w, y, x, y };
        public static float[] GenVertexSquare(int w, int h) => new float[] { -w, -h, 0, w, -h, 0, w, h, 0, -w, h, 0 };

        internal BufferObject CreateBuffer(float[] bufferData, string attributeName, int size)
        {
            uint buffer = Gl.GenBuffer();

            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(4 * bufferData.Length), bufferData, BufferUsage.StaticCopy);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0); 

            return new BufferObject { id = buffer, name = attributeName, size = size, data = bufferData };
        }

        internal void UpdateBuffer(Dictionary<uint, Transform> Transforms)
        {
            foreach (uint id in Transforms.Keys)
            {
                if (!_graphicsObjects.ContainsKey(id) || !_graphicsObjects[id].Visible) continue;

                float[] data = new float[12];
                Array.Copy(_graphicsObjects[id].Position.data, data, 12);

                for (int i = 0; i < data.Length; i += 3) {
                    Vector2 result = new Vector2(data[i], data[i + 1]) * Transforms[id].scale;
                    result += Transforms[id].position;
                    data[i] = result.X;
                    data[i + 1] = result.Y;
                }

                UpdateBuffer(_graphicsObjects[id].Position, data);
            }           
        }

        private void UpdateBuffer(BufferObject buffer, float[] bufferData)
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer.id);
            Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)(4 * bufferData.Length), bufferData);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
