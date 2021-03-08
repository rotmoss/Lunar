using Lunar.OpenGL;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.ECS 
{
    public abstract class RenderData : Component<RenderData> 
    {
        public abstract float[] Vertices { get; }
        public abstract uint[] Indices { get; }

        public Material Material { get => _material; }
        private Material _material;

        public bool Visible { get => _visible; set => _visible = value; }
        protected bool _visible;

        public abstract uint VertexCount { get; }
        protected VertexFormat _vertexFormat;

        private static Dictionary<Material, List<RenderData>> _renderDataByMaterial = new();

        public RenderData(Material material) : base()
        {
            Disposed += OnDispose;
            _visible = true;

            if(material == null) return;

            _material = material;
            _vertexFormat = material.ShaderProgram.VertexFormat;

            if (!_renderDataByMaterial.ContainsKey(material))
                _renderDataByMaterial.Add(material, new List<RenderData>());

            _renderDataByMaterial[material].Add(this);
        }

        public abstract void SetPosition(Transform transform);
        public abstract void SetColor(Vec4 color0, Vec4 color1, Vec4 color2, Vec4 color3);
        public abstract void SetTexCoords(Vec2 tex0, Vec2 tex1, Vec2 tex2, Vec2 tex3);
        public abstract void SetTexIds(float[] ids);

        public void UpdateTexIds() => SetTexIds(_material.TexIndex);

        public static void GetVertices(RenderData[] data, out float[] vertices, out uint[] indices) 
        {
            int v_size = 0, i_size = 0;

            for (int i = 0; i < data.Length; i++) 
            {
                v_size += data[i].Vertices.Length;
                i_size += data[i].Indices.Length;
            }

            vertices = new float[v_size];
            indices = new uint[i_size];

            uint indices_offset = 0;
            
            for (int i = 0; i < data.Length; i++)
            {
                data[i].SetPosition(Transform.GetLocalTransform(data[i].Id));
                data[i].UpdateTexIds();
            }

            for (int i = 0, v_index = 0, i_index = 0; i < data.Length; i++)
            {
                float[] v_temp = data[i].Vertices;

                for (int j = 0; j < v_temp.Length; j++, v_index++) 
                    vertices[v_index] = v_temp[j];

                uint[] i_temp = data[i].Indices;

                for (int j = 0; j < i_temp.Length; j++, i_index++) 
                    indices[i_index] = i_temp[j] + indices_offset;

                indices_offset += data[i].VertexCount;
            }
        }

        public static RenderData[] GetDataFromMaterials(params Material[] materials) 
        {
            int size = 0;
            for (int i = 0; i < materials.Length; i++) 
                size += _renderDataByMaterial[materials[i]].Count;

            RenderData[] result = new RenderData[size];
            
            for (int i = 0, index = 0; i < materials.Length; i++) {
                List<RenderData> data = _renderDataByMaterial[materials[i]];

                for (int j = 0; j < data.Count; j++, index++)
                    result[index] = data[j];
            }

            return result;
        }

        public void OnDispose()
        {
            if(Material != null) {
                _renderDataByMaterial[Material].Remove(this);
            }
        }
    }
}