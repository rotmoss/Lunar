using Lunar.ECS.Components;
using Lunar.GL;
using Lunar.SDL;
using OpenGL;
using System.Collections.Generic;

namespace Lunar.GL
{
    public class RenderToFrameBuffer : IRenderBehaviour
    {
        public FramebufferTexture Framebuffer { get => _framebuffer; }
        private FramebufferTexture _framebuffer;

        public List<string> RenderLayers { get => _renderLayers; }
        private List<string> _renderLayers = new List<string>(new string[] { "default" });

        public Dictionary<string, IShaderStorageBuffer> ShaderStorage { get => _shaderStorage; }
        public Dictionary<string, IShaderStorageBuffer> _shaderStorage;

        public float Ratio { get => _ratio; }
        private float _ratio = 16.0f / 9.0f;

        public bool Stretch { get => _stretch; }
        private bool _stretch;

        public RenderToFrameBuffer()
        {
            _stretch = false;

            _shaderStorage = new Dictionary<string, IShaderStorageBuffer>();
            _framebuffer = new FramebufferTexture("Framebuffer.vert", "Framebuffer.frag", 1280, 720, 1);

            _shaderStorage.Add("projection", new ShaderStorageBuffer<Matrix4x4f>(0, Matrix4x4f.Identity));
            _shaderStorage.Add("view", new ShaderStorageBuffer<Matrix4x4f>(1, Matrix4x4f.Identity));
            _shaderStorage.Add("aspectRatio", new ShaderStorageBuffer<float>(2, 1280.0f / 720.0f));
            _shaderStorage.Add("w", new ShaderStorageBuffer<float>(3, 1280));
            _shaderStorage.Add("h", new ShaderStorageBuffer<float>(4, 720));

            AddLayers("sprite", "background");
        }

        public void Initialize()
        {
            Gl.Initialize();
            Gl.BindAPI();

            Gl.ClearColor(0.2f, 0.2f, 0.2f, 1f);

            Gl.CullFace(CullFaceMode.Front);
            Gl.FrontFace(FrontFaceDirection.Cw);

            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.Enable(EnableCap.Blend);
        }

        public void AddLayers(params string[] value) => _renderLayers.AddRange(value);
        public void SetShaderStorage(object value) => _shaderStorage["view"].Data = value;

        public void Render()
        {
            Gl.Clear( ClearBufferMask.ColorBufferBit);

            _framebuffer.OpenBuffer();

            foreach (string layer in _renderLayers)
                foreach (GraphicsComponent renderData in GraphicsComponent.GetRenderData(layer))
                    renderData.Render();

            _framebuffer.CloseBuffer();
            _framebuffer.Render();
        }

        public void SetSize(ViewportSize size)
        {
            Matrix4x4d pMatrix = Matrix4x4d.Identity;
            float newRatio = size.W / size.H;

            if (_stretch) { pMatrix.Scale(1 / 1280.0, (1 / 720.0), 1); }
            else { pMatrix.Scale(1 / (1280.0 / (_ratio / newRatio)), 1 / 720.0, 1); }

            _shaderStorage["projection"].Data = (Matrix4x4f)pMatrix;
            _shaderStorage["aspectRatio"].Data = (float)newRatio;

            Framebuffer?.UpdateFrameSize(size.W, size.H);
        }

        public void Dispose()
        {
            ShaderProgram.DisposeShaders();
            _framebuffer.Dispose();
        }
    }
}
