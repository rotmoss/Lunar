using System;
using SDL2;
using OpenGL;
using System.Collections.Generic;
using System.Linq;
using Lunar.Scenes;
using Lunar.Graphics;

namespace Lunar
{
    public abstract class Window
    {
        protected static IntPtr _context;
        protected static IntPtr _window;

        public const float GameW = 1280;
        public const float GameH = 720;

        public static float Width { get => _width; set { _width = value > 0 ? (int)value : _width; } }
        protected static int _width;
        public static float Height { get => _height; set { _height = value > 0 ? (int)value : _height; } }
        protected static int _height;

        public static FramebufferTexture Framebuffer { get => _framebuffer; }
        protected static FramebufferTexture _framebuffer;
        public static List<string> RenderLayers { get => _renderLayers; }
        protected static List<string> _renderLayers = new List<string>(new string[] { "default" });

        public static ShaderStorageBuffer<Matrix4x4f> Projection { get => _projection; }
        protected static ShaderStorageBuffer<Matrix4x4f> _projection;

        public static ShaderStorageBuffer<Matrix4x4f> View { get => _view; }
        protected static ShaderStorageBuffer<Matrix4x4f> _view;

        public static ShaderStorageBuffer<float> AspectRatio { get => _aspectRatio; }
        protected static ShaderStorageBuffer<float> _aspectRatio;

        public static ShaderStorageBuffer<float> W { get => _w; }
        protected static ShaderStorageBuffer<float> _w;
        public static ShaderStorageBuffer<float> H { get => _h; }
        protected static ShaderStorageBuffer<float> _h;

        protected bool _stretch;
        protected float ASPECT_RATIO = 16.0f / 9.0f;

        public void UpdateWindowSize(object sender, EventArgs eventArgs) { SetViewport(); }
        protected abstract void SetViewport();

        public abstract void CreateWindowAndContext(SDL.SDL_WindowFlags flags);

        public static void CreateFramebuffer() => _framebuffer = new FramebufferTexture("Framebuffer.vert", "Framebuffer.frag", _width, _height, 1);
        public static void SwapBuffer() { SDL.SDL_GL_SwapWindow(_window); Gl.Clear(ClearBufferMask.ColorBufferBit); }

        public static void AddLayer(string value, int index) => _renderLayers.Insert(index, value);
        public static void AddLayers(params string[] value) => _renderLayers.AddRange(value);
        public static void SetLineWidth(int value) => Gl.LineWidth(value);
        public static void SetViewMatrix(Matrix4x4f value) => _view.Data = value;

        protected void UpdateProjectionMatrix()
        {
            Matrix4x4d pMatrix = Matrix4x4d.Identity;
            float newRatio = Width / Height;

            if (_stretch) { pMatrix.Scale(1 / GameW, (1 / GameH), 1); }
            else { pMatrix.Scale(1 / (GameW / (ASPECT_RATIO / newRatio)), 1 / GameH, 1); }

            _projection.Data = (Matrix4x4f)pMatrix;
            _aspectRatio.Data = (float)newRatio;
        }

        public static void Render()
        {
            _framebuffer.OpenBuffer();

            foreach (string layer in _renderLayers)
                foreach (GraphicsComponent renderData in GraphicsComponent.GetRenderData(layer))
                    renderData.Render();

            _framebuffer.CloseBuffer();
            _framebuffer.Render();
        }

        public static void Close()
        {
            GraphicsComponent.Close();
            ShaderProgram.DisposeShaders();
            _framebuffer.DisposeChild();

            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_GL_DeleteContext(_context);
            SDL.SDL_Quit();
            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
        }
    }
}
