using System;
using SDL2;
using OpenGL;

namespace Lunar
{
    class GLContext
    {
        public const float GameW = 3840;
        public const float GameH = 2160;

        private int _width;
        private int _height;

        private IntPtr _window;
        private IntPtr _context;

        private Matrix4x4f _scaling;
        public Matrix4x4f Scaling => _scaling;

        public float Width => _width;
        public float Height => _height;

        private bool _stretch;

        /// <summary> Initializes SDL and creates window and renderer. </summary>
        public GLContext(int w, int h, bool stretch)
        {
            _stretch = stretch;
            _width = w; _height = h;
        }

        public void Init()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            _window = SDL.SDL_CreateWindow("Game", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, _width, _height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            //_window = SDL.SDL_CreateWindow("Game", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, _width, _height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);

            Gl.Initialize();

            _context = SDL.SDL_GL_CreateContext(_window);

            SDL.SDL_GL_SetSwapInterval(1);
            Gl.ClearColor(0.2f, 0.2f, 0.2f, 1f);
            SetViewport(_width, _height);

            Gl.CullFace(CullFaceMode.Front);
            Gl.FrontFace(FrontFaceDirection.Cw);

            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.Enable(EnableCap.Blend);
        }

        private void SetViewport(float w, float h)
        {
            Gl.LoadIdentity();
            Gl.Viewport(0, 0, (int)w, (int)h);

            _scaling = Matrix4x4f.Identity;
            if (_stretch) { _scaling.Scale(1f / GameW, 1f / GameH, 1); }
            else { _scaling.Scale(1f / (GameW * (w / h) / 2), 1f / GameH, 1); }
        }

        public void UpdateWindowSize()
        {
            SDL.SDL_GetWindowSize(_window, out int _width, out int _height);
            SetViewport(_width, _height);
        }

        public void SwapBuffer()
        {
            SDL.SDL_GL_SwapWindow(_window);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Dispose()
        {
            SDL.SDL_GL_DeleteContext(_context);
        }
    }
}
