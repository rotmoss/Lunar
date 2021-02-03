using System;
using SDL2;
using OpenGL;
using Lunar.GL;
using Lunar.Input;
using Lunar.SDL;

namespace Lunar
{
    public static class Window
    {
        public static IRenderBehaviour RenderBehaviour { get => _renderBehaviour; }
        private static IRenderBehaviour _renderBehaviour;

        public static IWindowContext WindowContext { get => _windowContext; }
        private static IWindowContext _windowContext;

        public static void Init(IWindowContext windowContext, IRenderBehaviour renderBehaviour)
        {
            _windowContext = windowContext;
            _renderBehaviour = renderBehaviour;
            UpdateViewport(null, null);

            InputController.OnWindowSizeChanged += UpdateViewport;
            InputController.OnKeyDown += OnKeyDown;
        }

        public static void UpdateViewport(object sender, EventArgs e) => _renderBehaviour.SetSize(_windowContext.GetSize());
        public static void SwapBuffer() => _windowContext.SwapBuffer();
        public static void Render() => _renderBehaviour.Render();

        public static void SetSize(ViewportSize size) { _windowContext.SetSize(size); _renderBehaviour.SetSize(size);  }
        public static ViewportSize GetSize() => _windowContext.GetSize();      

        public static void Dispose()
        {
            _renderBehaviour.Dispose();
            _windowContext.Dispose();
            SDL2.SDL.SDL_Quit();
            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
        }

        public static void OnKeyDown(object sender, KeyboardState eventArgs)
        {
            if (eventArgs.RawKeyStates[SDL2.SDL.SDL_Keycode.SDLK_LALT] && eventArgs.RawKeyStates[SDL2.SDL.SDL_Keycode.SDLK_RETURN])
                _windowContext.ToggleFullscreen();
        }
    }
}
