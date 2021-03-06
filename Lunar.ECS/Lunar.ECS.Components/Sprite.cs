using Lunar.OpenGL;
using Silk.NET.Maths;
using Lunar;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Lunar.ECS
{
    public class Sprite : RenderData
    {
        public override int VertexCount { get => _vertexCount; }
        private int _vertexCount;

        public override float[] Vertices { get => _vertices; }
        private float[] _vertices;

        public override uint[] Indices { get => _indices; }
        private readonly uint[] _indices = new uint[6] { 0, 1, 2, 2, 3, 0 };

        public int Width { get => _width; }
        private int _width;

        public int Height { get => _height; }
        private int _height;

        public Sprite(Material material) : base(material)
        {         
            _width = material.Textures[0].Width;
            _height = material.Textures[0].Height;

            _vertexCount = 4;

            _vertices = new float[material.ShaderProgram.VertexFormat.TotalLength * 4];

            SetColor(new Vec4(1, 1, 1, 1), new Vec4(1, 1, 1, 1), new Vec4(1, 1, 1, 1), new Vec4(1, 1, 1, 1));
            SetTexCoords(new Vec2(0, 0), new Vec2(1, 0), new Vec2(1, 1), new Vec2(0, 1));
        }

        public override void SetPosition(Transform transform) 
        {
            VecInfo info = _vertexFormat.GetVecInfo(VecName.Position);
            
            if (info.Length == 0 || transform == null) return;

            float x = transform.Position.X;
            float y = transform.Position.Y;
            float z = transform.Position.Z;
            float w = transform.Scale.X * _width / 2;
            float h = transform.Scale.Y * _height / 2;

            int offset0 = info.OffsetLength;
            int offset1 = info.OffsetLength + (int)_vertexFormat.TotalLength;
            int offset2 = info.OffsetLength + (int)_vertexFormat.TotalLength * 2;
            int offset3 = info.OffsetLength + (int)_vertexFormat.TotalLength * 3;

            if (info.Length > 0)
            {
                _vertices[0 + offset0] = x - w;
                _vertices[0 + offset1] = x + w;
                _vertices[0 + offset2] = x + w;
                _vertices[0 + offset3] = x - w;
            }
            if (info.Length > 1)
            {
                _vertices[1 + offset0] = y - h;
                _vertices[1 + offset1] = y - h;
                _vertices[1 + offset2] = y + h;
                _vertices[1 + offset3] = y + h;
            }
            if (info.Length > 2)
            {
                _vertices[2 + offset0] = z;
                _vertices[2 + offset1] = z;
                _vertices[2 + offset2] = z;
                _vertices[2 + offset3] = z;
            }
            if (info.Length > 3)
            {
                _vertices[3 + offset0] = 1;
                _vertices[3 + offset1] = 1;
                _vertices[3 + offset2] = 1;
                _vertices[3 + offset3] = 1;
            }
        }

        public override void SetColor(Vec4 color0, Vec4 color1, Vec4 color2, Vec4 color3)
        {
            VecInfo info = _vertexFormat.GetVecInfo(VecName.Color);

            if (info.Length == 0) return;

            int offset0 = info.OffsetLength;
            int offset1 = info.OffsetLength + (int)_vertexFormat.TotalLength;
            int offset2 = info.OffsetLength + (int)_vertexFormat.TotalLength * 2;
            int offset3 = info.OffsetLength + (int)_vertexFormat.TotalLength * 3;

            if (info.Length > 0)
            {
                _vertices[0 + offset0] = color0.x;
                _vertices[0 + offset1] = color1.x;
                _vertices[0 + offset2] = color2.x;
                _vertices[0 + offset3] = color3.x;
            }
            if (info.Length > 1)
            {
                _vertices[1 + offset0] = color0.y;
                _vertices[1 + offset1] = color1.y;
                _vertices[1 + offset2] = color2.y;
                _vertices[1 + offset3] = color3.y;
            }
            if (info.Length > 2)
            {
                _vertices[2 + offset0] = color0.z;
                _vertices[2 + offset1] = color1.z;
                _vertices[2 + offset2] = color2.z;
                _vertices[2 + offset3] = color3.z;
            }
            if (info.Length > 3)
            {
                _vertices[3 + offset0] = color0.y;
                _vertices[3 + offset1] = color1.y;
                _vertices[3 + offset2] = color2.y;
                _vertices[3 + offset3] = color3.y;
            }
        }

        public override void SetTexCoords(Vec2 tex0, Vec2 tex1, Vec2 tex2, Vec2 tex3)
        {
            VecInfo info = _vertexFormat.GetVecInfo(VecName.TexCoord);

            if (info.Length == 0) return;

            int offset0 = info.OffsetLength;
            int offset1 = info.OffsetLength + (int)_vertexFormat.TotalLength;
            int offset2 = info.OffsetLength + (int)_vertexFormat.TotalLength * 2;
            int offset3 = info.OffsetLength + (int)_vertexFormat.TotalLength * 3;

            if (info.Length > 0)
            {
                _vertices[0 + offset0] = tex0.x;
                _vertices[0 + offset1] = tex1.x;
                _vertices[0 + offset2] = tex2.x;
                _vertices[0 + offset3] = tex3.x;
            }
            if (info.Length > 1)
            {
                _vertices[1 + offset0] = tex0.y;
                _vertices[1 + offset1] = tex1.y;
                _vertices[1 + offset2] = tex2.y;
                _vertices[1 + offset3] = tex3.y;
            }
            if (info.Length > 2)
            {
                _vertices[2 + offset0] = 0;
                _vertices[2 + offset1] = 0;
                _vertices[2 + offset2] = 0;
                _vertices[2 + offset3] = 0;
            }
            if (info.Length > 3)
            {
                _vertices[3 + offset0] = 0;
                _vertices[3 + offset1] = 0;
                _vertices[3 + offset2] = 0;
                _vertices[3 + offset3] = 0;
            }
        }

        public override void SetTexIds(float[] ids)
        {
            VecInfo info = _vertexFormat.GetVecInfo(VecName.TexIndex);

            if (info.Length == 0) return;

            int offset0 = info.OffsetLength;
            int offset1 = info.OffsetLength + (int)_vertexFormat.TotalLength;
            int offset2 = info.OffsetLength + (int)_vertexFormat.TotalLength * 2;
            int offset3 = info.OffsetLength + (int)_vertexFormat.TotalLength * 3;

            if (info.Length > 0 && ids.Length > 0)
            {
                _vertices[0 + offset0] = ids[0];
                _vertices[0 + offset1] = ids[0];
                _vertices[0 + offset2] = ids[0];
                _vertices[0 + offset3] = ids[0];
            }
            if (info.Length > 1 && ids.Length > 1)
            {
                _vertices[1 + offset0] = ids[1];
                _vertices[1 + offset1] = ids[1];
                _vertices[1 + offset2] = ids[1];
                _vertices[1 + offset3] = ids[1];
            }
            if (info.Length > 2 && ids.Length > 2)
            {
                _vertices[2 + offset0] = ids[2];
                _vertices[2 + offset1] = ids[2];
                _vertices[2 + offset2] = ids[2];
                _vertices[2 + offset3] = ids[2];
            }
            if (info.Length > 3 && ids.Length > 3)
            {
                _vertices[3 + offset0] = ids[3];
                _vertices[3 + offset1] = ids[3];
                _vertices[3 + offset2] = ids[3];
                _vertices[3 + offset3] = ids[3];
            }
        }
    }
}