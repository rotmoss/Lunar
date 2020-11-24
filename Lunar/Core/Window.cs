using System;
using SDL2;
using OpenGL;
using Lunar.Graphics;

namespace Lunar
{
    public abstract class Window : IDisposable
    {
        protected IntPtr _context;
        protected IntPtr _window;

        protected Renderer _renderer;

        public static float Window_w { get => _width; set { _width = value > 0 ? (int)value : _width; } }
        protected static int _width;

        public static float Window_h { get => _height; set { _height = value > 0 ? (int)value : _height; } }
        protected static int _height;

        public abstract void SetViewport();
        public abstract void CreateWindowAndContext(SDL.SDL_WindowFlags flags);

        public void UpdateSize(object sender, EventArgs eventArgs) => SetViewport();

        public void SwapBuffer() 
        { 
            SDL.SDL_GL_SwapWindow(_window);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Dispose()
        {
            Renderer.Close();
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_GL_DeleteContext(_context);
            SDL.SDL_Quit();
            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
        }
    }
}
