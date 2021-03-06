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

        public abstract int VertexCount { get; }
        protected VertexFormat _vertexFormat;

        private static Dictionary<Material, List<RenderData>> _renderDataByMaterial = new Dictionary<Material, List<RenderData>>();

        public RenderData(Material material) : base()
        {
            Disposed += OnDispose;
            _visible = true;
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

        public static void GetVertices(Material[] materials, out List<float> vertices, out List<uint> indices) 
        {
            vertices = new List<float>();
            indices = new List<uint>();
            int offset = 0;

            for (int i = 0; i < materials.Length; i++)
            {
                RenderData[] data = _renderDataByMaterial[materials[i]].Where(x => x._visible).ToArray();

                for (int x = 0; x < data.Length; x++)
                {
                    data[x].SetPosition(Transform.GetLocalTransform(data[x].Id));
                    data[x].SetTexIds(materials[i].TexIndex);
                }

                for (int x = 0; x < data.Length; x++)
                {
                    vertices.AddRange(data[x].Vertices);

                    for(int j = 0; j < data[x].Indices.Length; j++)
                        indices.Add((uint)(data[x].Indices[j] + offset));

                    offset += data[x].VertexCount;
                }
            }
        }

        public void OnDispose()
        {
            _renderDataByMaterial[Material].Remove(this);
        }
    }
}