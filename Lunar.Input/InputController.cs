using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static SDL2.SDL;

namespace Lunar.Input
{
    public static partial class InputController
    {
        private static SDL_Event _inputPolling;

        private static Keyboard _keyboard;
        public static EventHandler<KeyboardState> OnKeyDown;
        public static EventHandler<KeyboardState> OnKeyUp;

        private static List<GameController> _gameControllers;
        public static EventHandler<GameControllerState> OnButtonDown;
        public static EventHandler<GameControllerState> OnButtonUp;
        public static EventHandler<GameControllerState> OnAxisChange;

        public static EventHandler<EventArgs> OnWindowClose;
        public static EventHandler<EventArgs> OnWindowEnter;
        public static EventHandler<EventArgs> OnWindowExposed;
        public static EventHandler<EventArgs> OnWindowFocusGained;
        public static EventHandler<EventArgs> OnWindowFocusLost;
        public static EventHandler<EventArgs> OnWindowHidden;
        public static EventHandler<EventArgs> OnWindowLeave;
        public static EventHandler<EventArgs> OnWindowMinimized;
        public static EventHandler<EventArgs> OnWindowMaximized;
        public static EventHandler<EventArgs> OnWindowMoved;
        public static EventHandler<EventArgs> OnWindowResized;
        public static EventHandler<EventArgs> OnWindowRestored;
        public static EventHandler<EventArgs> OnWindowShown;
        public static EventHandler<EventArgs> OnWindowSizeChanged;
        public static EventHandler<EventArgs> OnWindowTakeFocus;

        public static void Init()
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

        public static bool GetKeyState(Key key) => _keyboard.ReadKeyState(key);
        public static bool GetKeyState(SDL_Keycode key) => _keyboard.ReadRawKeyState(key);
        public static bool GetButtonState(Button button, int id) => id < _gameControllers.Count ? _gameControllers[id].ReadButtonState(button) : false;
        public static double GetAxisState(Axis axis, int id) => id < _gameControllers.Count ? _gameControllers[id].ReadAxisState(axis) : 0;
        public static void PollInputs()
        {
            GameController controller;

            while (SDL_PollEvent(out _inputPolling) == 1)
            {
                if (_inputPolling.type == SDL_EventType.SDL_JOYBUTTONDOWN)
                {
                    controller = _gameControllers.Where(x => x.DeviceId == _inputPolling.jbutton.which).FirstOrDefault();
                    if (controller != null)
                    {
                        controller.ChangeButtonState((SDL_GameControllerButton)_inputPolling.jbutton.button, true);
                        OnButtonDown?.Invoke(null, controller.GetState());
                    }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_JOYBUTTONUP)
                {
                    controller = _gameControllers.Where(x => x.DeviceId == _inputPolling.jbutton.which).FirstOrDefault();
                    if (controller != null)
                    {
                        controller.ChangeButtonState((SDL_GameControllerButton)_inputPolling.jbutton.button, false);
                        OnButtonDown?.Invoke(null, controller.GetState());
                    }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_JOYAXISMOTION)
                {
                    controller = _gameControllers.Where(x => x.DeviceId == _inputPolling.jbutton.which).FirstOrDefault();
                    if (controller != null)
                    {
                        controller.ChangeAxisState((SDL_GameControllerAxis)_inputPolling.jaxis.axis, _inputPolling.jaxis.axisValue);
                        OnAxisChange?.Invoke(null, controller.GetState());
                    }
                }
                else if (_inputPolling.type == SDL_EventType.SDL_KEYDOWN)
                {
                    _keyboard.ChangeRawKeyState(_inputPolling.key.keysym.sym, true);
                    OnKeyDown?.Invoke(null, _keyboard.GetState());
                }
                else if (_inputPolling.type == SDL_EventType.SDL_KEYUP)
                {
                    _keyboard.ChangeRawKeyState(_inputPolling.key.keysym.sym, false);
                    OnKeyUp?.Invoke(null, _keyboard.GetState());
                }
                else if (_inputPolling.type == SDL_EventType.SDL_WINDOWEVENT)
                {
                    switch (_inputPolling.window.windowEvent)
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
