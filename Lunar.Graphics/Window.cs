using System;
using SDL2;
using OpenGL;
using System.Threading.Tasks;
using System.Numerics;
using Lunar.Scenes;

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

        public static Matrix4x4f Scaling => _scaling;
        private static Matrix4x4f _scaling;

        private static bool _stretch;
        private const float ASPECT_RATIO = 16.0f / 9.0f;

        public static void Init(int w, int h, bool fullScreen)
        {
            _width = w;
            _height = h;
            _fullscreen = fullScreen;
            _stretch = false;
            _scaling = Matrix4x4f.Identity;

            CreateWindowAndContext();
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
            SetViewport();

            Gl.CullFace(CullFaceMode.Front);
            Gl.FrontFace(FrontFaceDirection.Cw);

            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.Enable(EnableCap.Blend);
        }

        public static void ToggleFullscreen()
        {
            _fullscreen = !_fullscreen;
            SDL.SDL_SetWindowFullscreen(_window, _fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : (uint)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        }

        public static void DrawQuad(bool fill, float x1, float y1, float x2, float y2, byte r, byte b, byte g)
        {
            PrimitiveType type = fill ? PrimitiveType.Quads : PrimitiveType.LineStrip;
            Gl.Color3(r, g,b);
            Gl.Begin(type);
            Gl.Vertex2((x1 * Scaling.Row0.x) + (y1 * Scaling.Row0.y), (x1 * Scaling.Row1.x) + (y1 * Scaling.Row1.y));
            Gl.Vertex2((x1 * Scaling.Row0.x) + (y2 * Scaling.Row0.y), (x1 * Scaling.Row1.x) + (y2 * Scaling.Row1.y));
            Gl.Vertex2((x2 * Scaling.Row0.x) + (y2 * Scaling.Row0.y), (x1 * Scaling.Row1.x) + (y2 * Scaling.Row1.y));
            Gl.Vertex2((x2 * Scaling.Row0.x) + (y1 * Scaling.Row0.y), (x1 * Scaling.Row1.x) + (y1 * Scaling.Row1.y));
            Gl.Vertex2((x1 * Scaling.Row0.x) + (y1 * Scaling.Row0.y), (x1 * Scaling.Row1.x) + (y1 * Scaling.Row1.y));
            Gl.End();
        }
        private static void SetViewport()
        {
            Gl.LoadIdentity();
            Gl.Viewport(0, 0, _width, _height);

            _scaling = Matrix4x4f.Identity;
            float newRatio = Width / Height;
            if (_stretch) { _scaling.Scale(1f / GameW, 1f / GameH, 1); }
            else { _scaling.Scale(1f / (GameW / (ASPECT_RATIO / newRatio)), 1f / GameH, 1); }
        }

        public static Vector2 Scale(Vector2 v, Transform cam)
        {
            v *= cam.scale;
            v -= cam.position;

            Vertex4f temp = new Vertex4f(v.X, v.Y, 0, 1);
            temp = _scaling * temp;

            return new Vector2(temp.x, temp.y);
        }

        public static Vector2[] Scale(Vector2[] v, Transform cam)
        {
            for (int i = 0; i < v.Length; i++)
                v[i] = Scale(v[i], cam);

            return v;
        }

        public static void UpdateWindowSize()
        {
            SDL.SDL_GetWindowSize(_window, out _width, out _height);
            SetViewport();
        }

        public static void SwapBuffer()
        {
            SDL.SDL_GL_SwapWindow(_window);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public static void Close()
        {
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_GL_DeleteContext(_context);
            SDL.SDL_Quit();
            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
        }
    }
}
