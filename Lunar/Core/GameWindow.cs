using Lunar.Graphics;
using SDL2;
using System;
using OpenGL;

namespace Lunar
{
    public class GameWindow : Window
    {
        public static bool Fullscreen { get => _fullscreen; set => _fullscreen = value; }
        protected static bool _fullscreen;

        public GameWindow(int w, int h, bool fullScreen)
        {
            _width = w;
            _height = h;
            _fullscreen = fullScreen;

            SDL.SDL_WindowFlags flags;
            if (_fullscreen) flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
            else flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

            CreateWindowAndContext(flags);
            _renderer = new Renderer();
            
            SetViewport();
        }

        public override void CreateWindowAndContext(SDL.SDL_WindowFlags flags)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            Gl.Initialize();

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

        public override void SetViewport()
        {
            SDL.SDL_GetWindowSize(_window, out _width, out _height);

            Gl.LoadIdentity();
            Gl.Viewport(0, 0, _width, _height);

            _renderer.UpdateProjectionMatrix(_width, _height);
        }

        public void OnKeyDown(object sender, Input.KeyboardState eventArgs)
        {
            if (eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_LALT] && eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_RETURN])
            {
                _fullscreen = !_fullscreen;
                if (_fullscreen)
                {
                    SDL.SDL_GetDesktopDisplayMode(0, out SDL.SDL_DisplayMode mode);
                    _width = mode.w; _height = mode.h;
                    SDL.SDL_SetWindowSize(_window, _width, _height);
                    SDL.SDL_SetWindowFullscreen(_window, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
                    SetViewport();
                }
                else
                {
                    SDL.SDL_SetWindowFullscreen(_window, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                    SetViewport();
                }
            }
        }   
    }
}
