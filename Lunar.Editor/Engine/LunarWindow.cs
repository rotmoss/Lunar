using System;
using SDL2;
using static SDL2.SDL;
using Lunar.GL;
using System.Runtime.InteropServices;
using OpenGL;

namespace Lunar.Editor
{
    public class LunarWindow : Window
    {
        private IntPtr _winHandle;
        private IntPtr _handle;
        private int _x;
        private int _y;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowPos(IntPtr handle, IntPtr handleAfter, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr child, IntPtr newParent);
        [DllImport("user32.dll")]
        private static extern IntPtr ShowWindow(IntPtr handle, int command);
        [DllImport("user32.dll")]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public LunarWindow(IntPtr handle, int w, int h, int x, int y)
        {
            _handle = handle;

            _width = w;
            _height = h;
            _x = x;
            _y = y;

            CreateWindowAndContext(SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
            _renderer = new Renderer();

            SetViewport();
        }
        public override void CreateWindowAndContext(SDL_WindowFlags flags)
        {
            if (SDL_Init(SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL_GetError()); SDL_Quit(); }

            Gl.Initialize();

            _window = SDL_CreateWindow("Game", 0, 0, _width, _height, flags);
            _context = SDL_GL_CreateContext(_window);

            SDL_SysWMinfo info = new SDL_SysWMinfo();
            SDL_GetWindowWMInfo(_window, ref info);
            _winHandle = info.info.win.window;

            SetWindowPos(_winHandle, _handle, _x, _y, 0, 0, 0x0401);

            SetParent(_winHandle, _handle);
            ShowWindow(_winHandle, 1);

            SDL_GL_SetSwapInterval(1);
            Gl.ClearColor(0.2f, 0.2f, 0.2f, 1f);

            Gl.CullFace(CullFaceMode.Front);
            Gl.FrontFace(FrontFaceDirection.Cw);

            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.Enable(EnableCap.Blend);
        }

        public void Resize(int x, int y, int w, int h)
        {
            _width = w;
            _height = h;
            _x = x;
            _y = y;

            MoveWindow(_winHandle, _x, _y, _width, _height, true);
            SDL_SetWindowSize(_window, _width, _height);

            SetViewport();
        }

        public override void SetViewport()
        {
            Gl.LoadIdentity();
            Gl.Viewport(0, 0, _width, _height);

            _renderer.UpdateProjectionMatrix(_width, _height);
        }
    }
}
