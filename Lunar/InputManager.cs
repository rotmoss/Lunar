using System;
using System.Collections.Generic;
using SDL2;
using static SDL2.SDL;

namespace Lunar
{
    /// <summary> The type of keyboardevent </summary>
    public enum KeyEvent
    {
        ZERO, ONE, TWO, THREE, FOUR, FIVE,
        W, A, S, D,
        UP, DOWN, LEFT, RIGHT,
        SPACE, ENTER, SHIFT, R, ESC,
    }

    /// <summary> The type of windowevent </summary>
    public enum WindowEvent
    {
        EXIT, RESIZE
    }

    /// <summary> The type of Mouseevent </summary>
    public enum MouseEvent
    {
        L_CLICK, R_CLICK, WHEEL,
    }

    public class MouseEventArgs : EventArgs
    {
        public List<MouseEvent> EventTypes;
        public int wheel_direction;
        public int x;
        public int y;
    }

    public class WindowEventArgs : EventArgs
    {
        public List<WindowEvent> EventTypes;
    }
    public class KeyboardEventArgs : EventArgs
    {
        public List<KeyEvent> EventTypes;
    }

    public static class InputManager
    {
        public static event EventHandler<MouseEventArgs> MouseDown;
        public static event EventHandler<MouseEventArgs> MouseUp;
        public static event EventHandler<MouseEventArgs> MouseMove;
        public static event EventHandler<MouseEventArgs> MouseWheel;

        public static event EventHandler<KeyboardEventArgs> KeyDown;
        public static event EventHandler<KeyboardEventArgs> KeyUp;

        public static event EventHandler<WindowEventArgs> WindowChange;

        private static SDL_Event input_event;

        private static List<MouseEvent> mouseEvents = new List<MouseEvent>();
        private static List<KeyEvent> keyboardEvents = new List<KeyEvent>();
        private static List<WindowEvent> windowEvents = new List<WindowEvent>();

        private static int x;
        private static int y;

        public static void Clear()
        {
            if (MouseDown != null)
            {
                foreach (Delegate d in MouseDown.GetInvocationList())
                { MouseDown -= (EventHandler<MouseEventArgs>)d; }
            }
            if (MouseUp != null)
            {
                foreach (Delegate d in MouseUp.GetInvocationList())
                { MouseUp -= (EventHandler<MouseEventArgs>)d; }
            }
            if (MouseMove != null)
            {
                foreach (Delegate d in MouseMove.GetInvocationList())
                { MouseMove -= (EventHandler<MouseEventArgs>)d; }
            }
            if (KeyDown != null)
            {
                foreach (Delegate d in KeyDown.GetInvocationList())
                { KeyDown -= (EventHandler<KeyboardEventArgs>)d; }
            }
            if (KeyUp != null)
            {
                foreach (Delegate d in KeyUp.GetInvocationList())
                { KeyUp -= (EventHandler<KeyboardEventArgs>)d; }
            }
            if (WindowChange != null)
            {
                foreach (Delegate d in WindowChange.GetInvocationList())
                { WindowChange -= (EventHandler<WindowEventArgs>)d; }
            }
        }

        /// <summary> Checks if the state of any input devices has changed and invokes 
        /// apropriate events. </summary>
        /// <param name="instance"> The GameObject that will be affected by the event 
        /// invoke. inserting null makes the event affect all gameobjects. </param>
        public static void InvokeInputEvents(object instance)
        {
            mouseEvents.Remove(MouseEvent.WHEEL);
            windowEvents.Remove(WindowEvent.RESIZE);

            SDL_GetMouseState(out x, out y);

            while (SDL_PollEvent(out input_event) == 1)
            {
                switch (input_event.type)
                {
                    case SDL_EventType.SDL_WINDOWEVENT:
                        {
                            switch (input_event.window.windowEvent)
                            {
                                case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                                    SetEvent(windowEvents, WindowEvent.EXIT);
                                    break;

                                case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                    SetEvent(windowEvents, WindowEvent.RESIZE);
                                    break;
                            }

                            WindowEventArgs args = new WindowEventArgs();

                            args.EventTypes = windowEvents;
                            OnWindowCange(args, instance);
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEWHEEL:
                        {
                            MouseEventArgs args = new MouseEventArgs();

                            SetEvent(mouseEvents, MouseEvent.WHEEL);

                            args.EventTypes = mouseEvents;
                            args.wheel_direction = input_event.wheel.y;
                            OnMouseWheel(args, instance);
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        {
                            MouseEventArgs args = new MouseEventArgs();

                            switch (input_event.button.button)
                            {
                                case (byte)SDL_BUTTON_LEFT:
                                    SetEvent(mouseEvents, MouseEvent.L_CLICK);
                                    break;

                                case (byte)SDL_BUTTON_RIGHT:
                                    SetEvent(mouseEvents, MouseEvent.R_CLICK);
                                    break;
                            }

                            args.EventTypes = mouseEvents;
                            args.x = x;
                            args.y = y;
                            OnMouseDown(args, instance);
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        {
                            MouseEventArgs args = new MouseEventArgs();

                            switch (input_event.button.button)
                            {
                                case (byte)SDL_BUTTON_LEFT:
                                    RemoveEvent(mouseEvents, MouseEvent.L_CLICK);
                                    break;
                                case (byte)SDL_BUTTON_RIGHT:
                                    RemoveEvent(mouseEvents, MouseEvent.R_CLICK);
                                    break;
                            }

                            args.x = x;
                            args.y = y;
                            args.EventTypes = mouseEvents;
                            OnMouseUp(args, instance);
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEMOTION:
                        {
                            MouseEventArgs args = new MouseEventArgs();

                            args.x = x;
                            args.y = y;
                            OnMouseMove(args, instance);
                        }
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        {
                            KeyboardEventArgs args = new KeyboardEventArgs();

                            switch (input_event.key.keysym.sym)
                            {
                                case SDL_Keycode.SDLK_SPACE:
                                    SetEvent(keyboardEvents, KeyEvent.SPACE);
                                    break;

                                case SDL_Keycode.SDLK_LEFT:
                                    SetEvent(keyboardEvents, KeyEvent.LEFT);
                                    break;

                                case SDL_Keycode.SDLK_RIGHT:
                                    SetEvent(keyboardEvents, KeyEvent.RIGHT);
                                    break;

                                case SDL_Keycode.SDLK_UP:
                                    SetEvent(keyboardEvents, KeyEvent.UP);
                                    break;

                                case SDL_Keycode.SDLK_DOWN:
                                    SetEvent(keyboardEvents, KeyEvent.DOWN);
                                    break;

                                case SDL_Keycode.SDLK_a:
                                    SetEvent(keyboardEvents, KeyEvent.A);
                                    break;

                                case SDL_Keycode.SDLK_d:
                                    SetEvent(keyboardEvents, KeyEvent.D);
                                    break;

                                case SDL_Keycode.SDLK_w:
                                    SetEvent(keyboardEvents, KeyEvent.W);
                                    break;

                                case SDL_Keycode.SDLK_s:
                                    SetEvent(keyboardEvents, KeyEvent.S);
                                    break;

                                case SDL_Keycode.SDLK_RETURN:
                                    SetEvent(keyboardEvents, KeyEvent.ENTER);
                                    break;

                                case SDL_Keycode.SDLK_LSHIFT:
                                    SetEvent(keyboardEvents, KeyEvent.SHIFT);
                                    break;

                                case SDL_Keycode.SDLK_r:
                                    SetEvent(keyboardEvents, KeyEvent.R);
                                    break;

                                case SDL_Keycode.SDLK_ESCAPE:
                                    SetEvent(keyboardEvents, KeyEvent.ESC);
                                    break;
                                case SDL_Keycode.SDLK_1:
                                    SetEvent(keyboardEvents, KeyEvent.ONE);
                                    break;
                                case SDL_Keycode.SDLK_2:
                                    SetEvent(keyboardEvents, KeyEvent.TWO);
                                    break;
                                case SDL_Keycode.SDLK_3:
                                    SetEvent(keyboardEvents, KeyEvent.THREE);
                                    break;
                                case SDL_Keycode.SDLK_4:
                                    SetEvent(keyboardEvents, KeyEvent.FOUR);
                                    break;
                                case SDL_Keycode.SDLK_5:
                                    SetEvent(keyboardEvents, KeyEvent.FIVE);
                                    break;

                            }
                            args.EventTypes = keyboardEvents;
                            OnKeyDown(args, instance);
                        }
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        {
                            KeyboardEventArgs args = new KeyboardEventArgs();

                            switch (input_event.key.keysym.sym)
                            {
                                case SDL_Keycode.SDLK_SPACE:
                                    RemoveEvent(keyboardEvents, KeyEvent.SPACE);
                                    break;

                                case SDL_Keycode.SDLK_LEFT:
                                    RemoveEvent(keyboardEvents, KeyEvent.LEFT);
                                    break;

                                case SDL_Keycode.SDLK_RIGHT:
                                    RemoveEvent(keyboardEvents, KeyEvent.RIGHT);
                                    break;

                                case SDL_Keycode.SDLK_UP:
                                    RemoveEvent(keyboardEvents, KeyEvent.UP);
                                    break;

                                case SDL_Keycode.SDLK_DOWN:
                                    RemoveEvent(keyboardEvents, KeyEvent.DOWN);
                                    break;

                                case SDL_Keycode.SDLK_a:
                                    RemoveEvent(keyboardEvents, KeyEvent.A);
                                    break;

                                case SDL_Keycode.SDLK_d:
                                    RemoveEvent(keyboardEvents, KeyEvent.D);
                                    break;

                                case SDL_Keycode.SDLK_w:
                                    RemoveEvent(keyboardEvents, KeyEvent.W);
                                    break;

                                case SDL_Keycode.SDLK_s:
                                    RemoveEvent(keyboardEvents, KeyEvent.S);
                                    break;

                                case SDL_Keycode.SDLK_RETURN:
                                    RemoveEvent(keyboardEvents, KeyEvent.ENTER);
                                    break;

                                case SDL_Keycode.SDLK_LSHIFT:
                                    RemoveEvent(keyboardEvents, KeyEvent.SHIFT);
                                    break;

                                case SDL_Keycode.SDLK_r:
                                    RemoveEvent(keyboardEvents, KeyEvent.R);
                                    break;

                                case SDL_Keycode.SDLK_ESCAPE:
                                    RemoveEvent(keyboardEvents, KeyEvent.ESC);
                                    break;
                                case SDL_Keycode.SDLK_1:
                                    RemoveEvent(keyboardEvents, KeyEvent.ONE);
                                    break;
                                case SDL_Keycode.SDLK_2:
                                    RemoveEvent(keyboardEvents, KeyEvent.TWO);
                                    break;
                                case SDL_Keycode.SDLK_3:
                                    RemoveEvent(keyboardEvents, KeyEvent.THREE);
                                    break;
                                case SDL_Keycode.SDLK_4:
                                    RemoveEvent(keyboardEvents, KeyEvent.FOUR);
                                    break;
                                case SDL_Keycode.SDLK_5:
                                    RemoveEvent(keyboardEvents, KeyEvent.FIVE);
                                    break;
                            }
                            args.EventTypes = keyboardEvents;
                            OnKeyUp(args, instance);
                        }
                        break;
                }
            }
        }

        private static void OnMouseDown(MouseEventArgs e, object instance)
        {
            EventHandler<MouseEventArgs> handler = MouseDown;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (d.Target != instance && instance != null)
                { handler -= (EventHandler<MouseEventArgs>)d; }
            }

            handler?.Invoke(null, e);
        }

        private static void OnMouseUp(MouseEventArgs e, object instance)
        {
            EventHandler<MouseEventArgs> handler = MouseUp;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (d.Target != instance && instance != null)
                { handler -= (EventHandler<MouseEventArgs>)d; }
            }

            handler?.Invoke(null, e);
        }

        private static void OnMouseWheel(MouseEventArgs e, object instance)
        {
            EventHandler<MouseEventArgs> handler = MouseWheel;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (d.Target != instance && instance != null)
                { handler -= (EventHandler<MouseEventArgs>)d; }
            }

            handler?.Invoke(null, e);
        }

        private static void OnMouseMove(MouseEventArgs e, object instance)
        {
            EventHandler<MouseEventArgs> handler = MouseMove;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (d.Target != instance && instance != null)
                { handler -= (EventHandler<MouseEventArgs>)d; }
            }

            handler?.Invoke(null, e);
        }

        private static void OnKeyDown(KeyboardEventArgs e, object instance)
        {
            EventHandler<KeyboardEventArgs> handler = KeyDown;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (d.Target != instance && instance != null)
                { handler -= (EventHandler<KeyboardEventArgs>)d; }
            }

            handler?.Invoke(null, e);
        }

        private static void OnKeyUp(KeyboardEventArgs e, object instance)
        {
            EventHandler<KeyboardEventArgs> handler = KeyUp;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (d.Target != instance && instance != null)
                { handler -= (EventHandler<KeyboardEventArgs>)d; }
            }

            handler?.Invoke(null, e);
        }
        private static void OnWindowCange(WindowEventArgs e, object instance)
        {
            EventHandler<WindowEventArgs> handler = WindowChange;
            if (handler == null) { return; }

            foreach (Delegate d in handler.GetInvocationList())
            {
                if (d.Target != instance && instance != null)
                { handler -= (EventHandler<WindowEventArgs>)d; }
            }

            handler?.Invoke(null, e);
        }

        private static void SetEvent(List<MouseEvent> list, MouseEvent entry)
        { if (!list.Contains(entry)) { list.Add(entry); } }
        private static void RemoveEvent(List<MouseEvent> list, MouseEvent entry)
        { if (list.Contains(entry)) { list.Remove(entry); } }
        private static void SetEvent(List<WindowEvent> list, WindowEvent entry)
        { if (!list.Contains(entry)) { list.Add(entry); } }
        private static void RemoveEvent(List<WindowEvent> list, WindowEvent entry)
        { if (list.Contains(entry)) { list.Remove(entry); } }
        private static void SetEvent(List<KeyEvent> list, KeyEvent entry)
        { if (!list.Contains(entry)) { list.Add(entry); } }
        private static void RemoveEvent(List<KeyEvent> list, KeyEvent entry)
        { if (list.Contains(entry)) { list.Remove(entry); } }
    }
}