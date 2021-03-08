using System.Collections.Generic;
using Silk.NET.OpenGL;
using System.Linq;
using Lunar.ECS;

namespace Lunar.OpenGL
{
    public class Material
    {
        public Texture[] Textures { get => _textures; }
        private Texture[] _textures;

        public float[] TexIndex { get => _texIndex; }
        private float[] _texIndex;

        public ShaderProgram ShaderProgram { get => _shaderProgram; }
        private ShaderProgram _shaderProgram;

        private static List<Material> _materials = new List<Material>();
        private static Dictionary<ShaderProgram, List<Material>> _materialsByShader = new Dictionary<ShaderProgram, List<Material>>();

        private Material(ShaderProgram shaderProgram, params Texture[] textures) 
        {            
            _shaderProgram = shaderProgram;
            _textures = textures;
            _texIndex = null;

            if(!_materialsByShader.ContainsKey(shaderProgram))
                _materialsByShader.Add(shaderProgram, new List<Material>());

            _materialsByShader[shaderProgram].Add(this);
            _materials.Add(this);
        }

        public static Material CreateMaterial(ShaderProgram shaderProgram, params Texture[] textures)
        {
            if(shaderProgram == null) return null;

            foreach(Material m in _materials)
                if (m.Textures.SequenceEqual(textures) && m.ShaderProgram == shaderProgram)
                    return m;

            return new Material(shaderProgram, textures);
        }   

        public void SetTexIndexes(int offset, int length)
        {
            _texIndex = new float[length];

            for (int i = 0; i < length; i++)
                _texIndex[i] = i + offset;
        }

        public void BindTexture() 
        {
            for (int i = 0; i < _texIndex.Length; i++)
                Engine.GL.BindTextureUnit((uint)_texIndex[i], _textures[i].Id);
        }

        public static List<Material[]> GetMaterials(ShaderProgram shaderProgram)
        {
            if (!_materialsByShader.ContainsKey(shaderProgram)) return null;

            List<Material> source = _materialsByShader[shaderProgram];
            List<Material[]> result = new List<Material[]>();

            List<Material> temp = new List<Material>();
            int texCount = 0;
            int texLength = shaderProgram.VertexFormat.GetVecInfo(VecName.TexIndex).Length;

            for (int i = 0; i < source.Count; i++)
            {
                if(texCount + texLength > 31)
                {
                    result.Add(temp.ToArray());
                    temp.Clear();
                    texCount = 0;
                }

                source[i].SetTexIndexes(texCount, texLength);
                temp.Add(source[i]);
                texCount += texLength;
            }

            result.Add(temp.ToArray());

            return result;
        }
    }
}