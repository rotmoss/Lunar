using System.Collections.Generic;
using Silk.NET.OpenGL;
using System.Linq;

namespace Lunar.OpenGL
{
    public enum VecName {
        Position = 0,
        Color = 1,
        TexCoord = 2,
        TexIndex = 3
    }

    public struct VecInfo 
    {
        public VecName Name;
        public int AttribLocation;
        public int Size;
        public int Length;
        public int OffsetBytes;
        public int OffsetLength;

        public VecInfo(int location, int length) 
        {
            Name = (VecName)location;

            if(length == 1) Size = 4;
            else if(length == 2) Size = 8;
            else if(length == 3) Size = 12;
            else if(length == 4) Size = 16;
            else Size = 0;

            Length = length;
            AttribLocation = location;
            OffsetBytes = 0;
            OffsetLength = 0;
        }

        public void SetOffsetBytes(int offset) => OffsetBytes = offset;
        public void SetOffsetLength(int offset) => OffsetLength = offset;
    }

    public struct VertexFormat
    {
        private VecInfo[] _vecs;
        private Dictionary<VecName, VecInfo> _vecByName;

        public VertexFormat(VecInfo[] vecs) 
        {
            _vecByName = new Dictionary<VecName, VecInfo>();
            _vecs = vecs;

            uint result = 0; 
            for (int i = 0; i < _vecs.Length; i++) 
                result += (uint)_vecs[i].Size;

            _totalSize = result;
            _totalLength = _totalSize / 4;

            for (int i = 0; i < vecs.Length; i++)
                _vecByName.Add(vecs[i].Name, vecs[i]);
        }

        public int Count { get => _vecs.Length; }

        public uint TotalSize { get => _totalSize; }
        private uint _totalSize;

        public uint TotalLength { get => _totalLength; }
        private uint _totalLength;

        public bool Contains(VecName name) => _vecByName.ContainsKey(name);
        public VecInfo GetVecInfo(VecName name) => _vecByName.ContainsKey(name) ? _vecByName[name] : default; 
        public VecName Name(int index) => _vecs[index].Name;
        public int AttribLocation(int index) => _vecs[index].AttribLocation;
        public int Size(int index) => _vecs[index].Size;
        public int Length(int index) => _vecs[index].Length;
        public int OffsetBytes(int index) => _vecs[index].OffsetBytes;
        public int OffsetLength(int index) => _vecs[index].OffsetLength;
    }
}