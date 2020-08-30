using System;
using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Lunar
{
    public partial class InputController
    {
        private static InputController instance = null;
        public static InputController Instance { get { instance = instance == null ? new InputController() : instance; return instance; } }

        private SDL_Event _inputPolling;

        private Keyboard _keyboard;
        public EventHandler<EventArgs> OnKeyDown;
        public EventHandler<EventArgs> OnKeyUp;

        private List<GameController> _gameControllers;
        public EventHandler<EventArgs> OnButtonDown;
        public EventHandler<EventArgs> OnButtonUp;
        public EventHandler<EventArgs> OnAxisChange;

        public EventHandler<EventArgs> OnWindowClose;
        public EventHandler<EventArgs> OnWindowEnter;
        public EventHandler<EventArgs> OnWindowExposed;
        public EventHandler<EventArgs> OnWindowFocusGained;
        public EventHandler<EventArgs> OnWindowFocusLost;
        public EventHandler<EventArgs> OnWindowHidden;
        public EventHandler<EventArgs> OnWindowLeave;
        public EventHandler<EventArgs> OnWindowMinimized;
        public EventHandler<EventArgs> OnWindowMaximized;
        public EventHandler<EventArgs> OnWindowMoved;
        public EventHandler<EventArgs> OnWindowResized;
        public EventHandler<EventArgs> OnWindowRestored;
        public EventHandler<EventArgs> OnWindowShown;
        public EventHandler<EventArgs> OnWindowSizeChanged;
        public EventHandler<EventArgs> OnWindowTakeFocus;

        public InputController()
        {
            _gameControllers = new List<GameController>();

            //Add Keyboard to InputDevices
            _keyboard = new Keyboard();

            //Check for joysticks
            if (SDL_NumJoysticks() < 1) Console.WriteLine("No joysticks connected!");
            for (int i = 0; i < SDL_NumJoysticks(); i++)
            {
                //Add joysticks to InputDevices
                if (SDL_JoystickOpen(i) == IntPtr.Zero) Console.WriteLine("Warning: Unable to open game controller! SDL Error: ", SDL_GetError());
                else { _gameControllers.Add(new GameController(SDL_JoystickOpen(i))); }
            }
        }

        public bool KeyDown(Key key) => _keyboard.ReadKeyState(key);
        public bool KeyDown(SDL_Keycode key) => _keyboard.ReadRawKeyState(key);

        public void PollInputs()
        {
            GameController controller;

            while (SDL_PollEvent(out _inputPolling) == 1)
            {
                if (_inputPolling.type == SDL_EventType.SDL_JOYBUTTONDOWN)
                {
                    controller = _gameControllers.Where(x => x.DeviceId == _inputPolling.jbutton.which).FirstOrDefault();
                    if (controller != null) {
                        GameControllerState state = controller.OnButtonStateChange((SDL_GameControllerButton)_inputPolling.jbutton.button, _inputPolling.type);
                        if (state != null)OnButtonDown?.Invoke(null, state);
                    }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_JOYBUTTONUP)
                {
                    controller = _gameControllers.Where(x => x.DeviceId == _inputPolling.jbutton.which).FirstOrDefault();
                    if (controller != null)  {
                        GameControllerState state = controller.OnButtonStateChange((SDL_GameControllerButton)_inputPolling.jbutton.button, _inputPolling.type);
                        if (state != null) OnButtonDown?.Invoke(null, state);
                    }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_JOYAXISMOTION)
                {
                    controller = _gameControllers.Where(x => x.DeviceId == _inputPolling.jbutton.which).FirstOrDefault();
                    if (controller != null) {
                        GameControllerState state = controller.OnAxisStateChange((SDL_GameControllerAxis)_inputPolling.jaxis.axis, _inputPolling.jaxis.axisValue);
                        if (state != null) OnAxisChange?.Invoke(null, state);
                    }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_KEYDOWN) {
                    KeyboardState state = _keyboard.ChangeKeyState(_inputPolling.key.keysym.sym, _inputPolling.type);
                    if (state != null) { OnKeyDown?.Invoke(null, state);  }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_KEYUP) {
                    KeyboardState state = _keyboard.ChangeKeyState(_inputPolling.key.keysym.sym, _inputPolling.type);
                    if (state != null) { OnKeyUp?.Invoke(null, state); }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_WINDOWEVENT) {
                    switch(_inputPolling.window.windowEvent)
                    {
                        case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE: OnWindowClose?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER: OnWindowEnter?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED: OnWindowExposed?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED: OnWindowFocusGained?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST: OnWindowFocusLost?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN: OnWindowHidden?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE: OnWindowLeave?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED: OnWindowMaximized?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED: OnWindowMinimized?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED: OnWindowMoved?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED: OnWindowResized?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED: OnWindowRestored?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN: OnWindowShown?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED: OnWindowSizeChanged?.Invoke(null, new EventArgs()); break;
                        case SDL_WindowEventID.SDL_WINDOWEVENT_TAKE_FOCUS: OnWindowTakeFocus?.Invoke(null, new EventArgs()); break;
                    }
                }
            }
        }
    }
}
