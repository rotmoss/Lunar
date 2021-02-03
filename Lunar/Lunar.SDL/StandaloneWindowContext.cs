using System;
using SDL2;

namespace Lunar.SDL
{
    public class StandaloneWindowContext : IWindowContext
    {
        IntPtr _window;
        IntPtr _context;

        public static bool Fullscreen { get => _fullscreen; set => _fullscreen = value; }
        protected static bool _fullscreen;

        public StandaloneWindowContext(ViewportSize size)
        {
            if (SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL2.SDL.SDL_GetError()); SDL2.SDL.SDL_Quit(); }

            _window = SDL2.SDL.SDL_CreateWindow("Game", SDL2.SDL.SDL_WINDOWPOS_UNDEFINED, SDL2.SDL.SDL_WINDOWPOS_UNDEFINED, size.W, size.H, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
            _context = SDL2.SDL.SDL_GL_CreateContext(_window);

            SDL2.SDL.SDL_GL_SetSwapInterval(1);
        }

        public void SetSize(ViewportSize rect)
        {
            SDL2.SDL.SDL_SetWindowSize(_window, rect.W, rect.H);
        }

        public ViewportSize GetSize()
        {
            SDL2.SDL.SDL_GetWindowSize(_window, out int w, out int h);
            return new ViewportSize() { W = w, H = h };
        }

        public void ToggleFullscreen()
        {
            _fullscreen = !_fullscreen;
            if (_fullscreen)
            {
                SDL2.SDL.SDL_GetDesktopDisplayMode(0, out SDL2.SDL.SDL_DisplayMode mode);
                SetSize(new ViewportSize() { W = mode.w, H = mode.h });

                SDL2.SDL.SDL_SetWindowFullscreen(_window, (uint)SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
            }
            else
            {
                SDL2.SDL.SDL_SetWindowFullscreen(_window, (uint)SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                SetSize(GetSize());
            }
        }

        public void SwapBuffer()
        {
            SDL2.SDL.SDL_GL_SwapWindow(_window);
        }

        public void Dispose()
        {
            SDL2.SDL.SDL_DestroyWindow(_window);
            SDL2.SDL.SDL_GL_DeleteContext(_context);
        }
    }
}
