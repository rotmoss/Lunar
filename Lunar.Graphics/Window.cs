using System;
using SDL2;
using OpenGL;
using System.Collections.Generic;
using System.Linq;
using Lunar.Scenes;
using Lunar.Transforms;

namespace Lunar.Graphics
{
    public static class Window
    {
        private static IntPtr _context;
        private static IntPtr _window;

        public const float GameW = 1280;
        public const float GameH = 720;

        public static float Width { get => _width; set { _width = value > 0 ? (int)value : _width; } }
        private static int _width;
        public static float Height { get => _height; set { _height = value > 0 ? (int)value : _height; } }
        private static int _height;
        public static bool Fullscreen { get => _fullscreen; set => _fullscreen = value; }
        private static bool _fullscreen;

        public static FramebufferTexture Framebuffer { get => _framebuffer; }
        private static FramebufferTexture _framebuffer;

        public static List<RenderData> _renderData = new List<RenderData>();
        public static List<string> RenderLayers { get => _renderLayers; }
        public static List<string> _renderLayers = new List<string>(new string[] { "default" });

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

        private static bool _stretch;
        private const float ASPECT_RATIO = 16.0f / 9.0f;

        public static void Init(int w, int h, bool fullScreen)
        {
            _width = w;
            _height = h;
            _fullscreen = fullScreen;
            _stretch = false;

            CreateWindowAndContext();

            _projection = new ShaderStorageBuffer<Matrix4x4f>(0, Matrix4x4f.Identity);
            _view = new ShaderStorageBuffer<Matrix4x4f>(1, Matrix4x4f.Identity);
            _aspectRatio = new ShaderStorageBuffer<float>(2, ASPECT_RATIO);
            _w = new ShaderStorageBuffer<float>(3, GameW);
            _h = new ShaderStorageBuffer<float>(4, GameH);

            SetViewport();
        }

        public static void CreateWindowAndContext()
        {
            Gl.Initialize();

            SDL.SDL_WindowFlags flags;
            if (_fullscreen) flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
            else flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

            _window = SDL.SDL_CreateWindow("Game", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, _width, _height, flags);
            _context = SDL.SDL_GL_CreateContext(_window);

            SDL.SDL_GL_SetSwapInterval(1);
            Gl.ClearColor(0.2f, 0.2f, 0.2f, 1f);       

            Gl.CullFace(CullFaceMode.Front);
            Gl.FrontFace(FrontFaceDirection.Cw);

            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.Enable(EnableCap.Blend);
        }

        public static void ToggleFullscreen()
        {
            _fullscreen = !_fullscreen;
            SDL.SDL_GetDesktopDisplayMode(0, out SDL.SDL_DisplayMode mode);
            _width = mode.w;
            _height = mode.h;
            SDL.SDL_SetWindowSize(_window, _width, _height);
            SetViewport();
            SDL.SDL_SetWindowFullscreen(_window, _fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : (uint)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        }

        public static void UpdateWindowSize()
        {
            SDL.SDL_GetWindowSize(_window, out _width, out _height);
            SetViewport();
        }

        private static void SetViewport()
        {
            Gl.LoadIdentity();
            Gl.Viewport(0, 0, _width, _height);

            Matrix4x4d temp = Matrix4x4d.Identity;
            float newRatio = Width / Height;
            if (_stretch) { temp.Scale(1 / GameW, (1 / GameH), 1); }
            else { temp.Scale(1 / (GameW / (ASPECT_RATIO / newRatio)), 1 / GameH, 1); }

            _projection.Data = (Matrix4x4f)temp;
            _aspectRatio.Data = (float)newRatio;
            _w.Data = GameW;
            _h.Data = GameH;


            Framebuffer?.UpdateFrameSize(_width, _height);
        }

        public static void SwapBuffer()
        {
            SDL.SDL_GL_SwapWindow(_window);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public static void TranslateBuffers(string attributeName)
        {
            foreach (RenderData g in _renderData)
                g.TranslateBuffer("aPos", Transform.GetGlobalTransform(g.id));
        }

        public static void Render()
        {
            _framebuffer.OpenBuffer();

            foreach (string layer in _renderLayers)
                foreach (RenderData renderData in _renderData.Where(x => x.Layer == layer))
                    renderData.Render();

            _framebuffer.CloseBuffer();
            _framebuffer.Render();
        }
        public static void Close()
        {
            while (_renderData.Count > 0)
                _renderData[0].Dispose();

            ShaderProgram.DisposeShaders();
            _framebuffer.Dispose();

            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_GL_DeleteContext(_context);
            SDL.SDL_Quit();
            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
        }

        public static void CreateFramebuffer() => _framebuffer = new FramebufferTexture(_width, _height, 1, "Framebuffer.vert", "Framebuffer.frag");
        public static void AddRenderData(RenderData value)
        {
            _renderData.Add(value);

            Scene.GetScene(value.id).OnSceneDispose += OnSceneDispose;
        }
        public static void RemoveRenderData(RenderData value) => _renderData.Remove(value);
        public static void AddLayer(string value, int index) => _renderLayers.Insert(index, value);
        public static void AddLayers(params string[] value) => _renderLayers.AddRange(value);
        public static void SetLineWidth(int value) => Gl.LineWidth(value);
        public static void SetViewMatrix(Matrix4x4f value) => _view.Data = value;
        public static void OnSceneDispose(object sender, DisposedEventArgs e)
        {
            for (int i = 0; i < e.Ids.Count; i++)
            {
                IList<RenderData> data = _renderData.Where(x => x.id == e.Ids[i]).ToList();
                foreach (RenderData d in data) { d.Dispose(); } 
            }
        }
    }
}
