using System.Collections.Generic;
using OpenGL;

namespace Lunar.Graphics
{
    public class Renderer
    {
        public static FramebufferTexture Framebuffer { get => _framebuffer; }
        private static FramebufferTexture _framebuffer;

        public static List<string> RenderLayers { get => _renderLayers; }
        private static List<string> _renderLayers = new List<string>(new string[] { "default" });

        public static ShaderStorageBuffer<Matrix4x4f> Projection { get => _projection; }
        private static ShaderStorageBuffer<Matrix4x4f> _projection;

        public static ShaderStorageBuffer<Matrix4x4f> View { get => _view; }
        private static ShaderStorageBuffer<Matrix4x4f> _view;

        public static ShaderStorageBuffer<float> AspectRatio { get => _aspectRatio; }
        private static ShaderStorageBuffer<float> _aspectRatio;

        public static ShaderStorageBuffer<float> W { get => _w; }
        private static ShaderStorageBuffer<float> _w;

        public static ShaderStorageBuffer<float> H { get => _h; }
        private static ShaderStorageBuffer<float> _h;

        private float _ratio = 16.0f / 9.0f;
        private bool _stretch;

        public Renderer()
        {
            _stretch = false;
            _framebuffer = new FramebufferTexture("Framebuffer.vert", "Framebuffer.frag", 1280, 720, 1);
            _projection = new ShaderStorageBuffer<Matrix4x4f>(0, Matrix4x4f.Identity);
            _view = new ShaderStorageBuffer<Matrix4x4f>(1, Matrix4x4f.Identity);
            _aspectRatio = new ShaderStorageBuffer<float>(2, 1280.0f / 720.0f);
            _w = new ShaderStorageBuffer<float>(3, 1280);
            _h = new ShaderStorageBuffer<float>(4, 720);
        }

        public static void AddLayer(string value, int index) => _renderLayers.Insert(index, value);
        public static void AddLayers(params string[] value) => _renderLayers.AddRange(value);
        public static void SetLineWidth(int value) => Gl.LineWidth(value);
        public static void SetViewMatrix(Matrix4x4f value) => _view.Data = value;

        public static void Render()
        {
            _framebuffer.OpenBuffer();

            foreach (string layer in _renderLayers)
                foreach (GraphicsComponent renderData in GraphicsComponent.GetRenderData(layer))
                    renderData.Render();

            _framebuffer.CloseBuffer();
            _framebuffer.Render();
        }

        public void UpdateProjectionMatrix(float w, float h)
        {
            Matrix4x4d pMatrix = Matrix4x4d.Identity;
            float newRatio = w / h;

            if (_stretch) { pMatrix.Scale(1 / 1280.0, (1 / 720.0), 1); }
            else { pMatrix.Scale(1 / (1280.0 / (_ratio / newRatio)), 1 / 720.0, 1); }

            _projection.Data = (Matrix4x4f)pMatrix;
            _aspectRatio.Data = (float)newRatio;

            Framebuffer?.UpdateFrameSize((int)w, (int)h);
        }

        public static void Close()
        {
            GraphicsComponent.Close();
            ShaderProgram.DisposeShaders();
            _framebuffer.DisposeChild();
        }
    }
}
