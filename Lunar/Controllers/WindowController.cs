using OpenGL;
using SDL2;
using System;

namespace Lunar
{
    class WindowController
    {
        private static WindowController instance = null;
        public static WindowController Instance { get { instance ??= new WindowController(); return instance; } }

        private IntPtr _context;
        private IntPtr _window;

        public const float GameW = 1280;
        public const float GameH = 720;

        public float Width { get => _width; set { _width = value > 0 ? (int)value : _width; } }
        private int _width;
        public float Height { get => _height; set { _height = value > 0 ? (int)value : _height; } }
        private int _height;
        public bool Fullscreen { get => _fullscreen; set => _fullscreen = value; }
        private bool _fullscreen;

        public Matrix4x4f Scaling => _scaling;
        private Matrix4x4f _scaling;

        private bool _stretch;
        private const float ASPECT_RATIO = 16.0f / 9.0f;

        /// <summary> Initializes SDL and creates window and renderer. </summary>
        public WindowController()
        {
            _height = 0;
            _width = 0;
            _fullscreen = false;
            _stretch = false;
        }

        internal void Init()
        {
            Gl.Initialize();
        }

        internal void CreateWindowAndContext()
        {
            SDL.SDL_WindowFlags flags;
            if (_fullscreen) flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
            else flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

            if (_window != IntPtr.Zero) { SDL.SDL_DestroyWindow(_window); _window = IntPtr.Zero; }
            if (_context != IntPtr.Zero) { SDL.SDL_GL_DeleteContext(_context); _context = IntPtr.Zero; }

            _window = SDL.SDL_CreateWindow("Game", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, _width, _height, flags);
            _context = SDL.SDL_GL_CreateContext(_window);

            SDL.SDL_GL_SetSwapInterval(1);
            Gl.ClearColor(0.2f, 0.2f, 0.2f, 1f);
            SetViewport();

            Gl.CullFace(CullFaceMode.Front);
            Gl.FrontFace(FrontFaceDirection.Cw);

            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.Enable(EnableCap.Blend);
        }

        private void SetViewport()
        {
            Gl.LoadIdentity();
            Gl.Viewport(0, 0, _width, _height);

            _scaling = Matrix4x4f.Identity;
            float newRatio = Width / Height;
            if (_stretch) { _scaling.Scale(1f / GameW, 1f / GameH, 1); }
            else { _scaling.Scale(1f / (GameW / (ASPECT_RATIO / newRatio)), 1f / GameH, 1); }
        }

        internal void UpdateWindowSize()
        {
            SDL.SDL_GetWindowSize(_window, out _width, out _height);
            SetViewport();
        }

        internal void SwapBuffer()
        {
            SDL.SDL_GL_SwapWindow(_window);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        internal void Dispose()
        {
            SDL.SDL_Quit();
            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
            SDL.SDL_GL_DeleteContext(_context);
        }
    }
}
