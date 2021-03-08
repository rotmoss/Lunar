using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;
using System.Linq;

namespace Lunar.OpenGL
{
    public class VertexArrayObject
    {
        private uint _arrayBuffer;
        private uint _indexBuffer;

        public uint Id { get => _id; }
        private uint _id;

        public static List<VertexArrayObject> VAOs { get => _vertexArrays; }
        private static List<VertexArrayObject> _vertexArrays = new List<VertexArrayObject>();

        private static Dictionary<ShaderProgram, VertexArrayObject> _vertexArrayByShader = new Dictionary<ShaderProgram, VertexArrayObject>();
        public static VertexArrayObject GetVAOByShader(ShaderProgram shaderProgram) => _vertexArrayByShader.ContainsKey(shaderProgram) ? _vertexArrayByShader[shaderProgram] : null;

        private unsafe VertexArrayObject(ShaderProgram shaderProgram)
        {
            _id = Engine.GL.GenVertexArray();
            Engine.GL.BindVertexArray(_id);
            
            _arrayBuffer = Engine.GL.GenBuffer();
            _indexBuffer = Engine.GL.GenBuffer();

            uint bufferSize = shaderProgram.VertexFormat.TotalSize;

            Engine.GL.BindBuffer(BufferTargetARB.ArrayBuffer, _arrayBuffer);
            Engine.GL.BufferData(BufferTargetARB.ArrayBuffer, bufferSize * 100000, null, BufferUsageARB.DynamicDraw);
            
            Engine.GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, _indexBuffer);
            Engine.GL.BufferData(BufferTargetARB.ElementArrayBuffer, 150000, null, BufferUsageARB.DynamicDraw);

            for (int i = 0; i < shaderProgram.VertexFormat.Count; i++) {
                Engine.GL.EnableVertexArrayAttrib(_id, (uint)shaderProgram.VertexFormat.AttribLocation(i));
                Engine.GL.VertexAttribPointer(
                    (uint)shaderProgram.VertexFormat.AttribLocation(i), 
                    shaderProgram.VertexFormat.Length(i), 
                    GLEnum.Float, 
                    false, 
                    bufferSize, 
                    (void*)shaderProgram.VertexFormat.OffsetBytes(i));
            }
        }

        public static unsafe void CreateVertexArray(ShaderProgram shaderProgram)
        {
            if(_vertexArrayByShader.ContainsKey(shaderProgram)) return;

            VertexArrayObject VAO = new VertexArrayObject(shaderProgram);

            _vertexArrayByShader.Add(shaderProgram, VAO);
            VAOs.Add(VAO);
        }   

        public unsafe void Draw(float[] vertices, uint[] indices)
        {
            Engine.GL.BindBuffer(BufferTargetARB.ArrayBuffer, _arrayBuffer);
            fixed(void* dataPointer = &vertices[0]) {
                Engine.GL.BufferSubData(GLEnum.ArrayBuffer, 0, (nuint)(vertices.Length * 4), dataPointer);
            }

            Engine.GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, _indexBuffer);
            fixed (void* dataPointer = &indices[0]) {
                Engine.GL.BufferSubData(GLEnum.ElementArrayBuffer, 0, (nuint)(indices.Length * 4), dataPointer);
            }

            Engine.GL.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, GLEnum.UnsignedInt, null);
        }

        public static void Dispose() 
        {
            foreach (VertexArrayObject VAO in _vertexArrays)
                Engine.GL.DeleteVertexArray(VAO.Id);           

            _vertexArrays.Clear();
            _vertexArrayByShader.Clear();
        }
    }
}