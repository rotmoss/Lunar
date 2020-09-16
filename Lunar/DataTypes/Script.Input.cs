using System;

namespace Lunar
{
    public partial class Script
    {
        public bool GetKeyState(Key key) => InputController.Instance.GetKeyState(key);
        public bool GetKeyState(SDL2.SDL.SDL_Keycode key) => InputController.Instance.GetKeyState(key);
        public bool GetButtonState(Button button, int id) => InputController.Instance.GetButtonState(button, id);
        public float GetAxisState(Axis axsis, int id) => InputController.Instance.GetAxisState(axsis, id);
    }
}
