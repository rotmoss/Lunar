using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Lunar
{
    public class GameControllerState : EventArgs
    {
        public readonly Dictionary<Button, bool> ButtonStates;
        public readonly Dictionary<Axis, float> AxisStates;

        public GameControllerState(Dictionary<Axis, float> axisStates, Dictionary<Button, bool> buttonStates)
        {
            AxisStates = axisStates == null ? new Dictionary<Axis, float>() : axisStates;
            ButtonStates = buttonStates == null ? new Dictionary<Button, bool>() : buttonStates;
        }
    }

    public class GameController
    {
        private IntPtr _controller;

        private Dictionary<SDL_GameControllerButton, Button> _buttonMap;
        private Dictionary<SDL_GameControllerAxis, Axis> _axisMap;

        private Dictionary<Button, bool> _buttonStates;
        private Dictionary<Axis, float> _axisStates;

        public readonly int DeviceId;

        public GameController(IntPtr controller, Dictionary<SDL_GameControllerButton, Button> buttonMap = null, Dictionary<SDL_GameControllerAxis, Axis> axisMap = null)
        {
            _controller = controller;

            _buttonMap = buttonMap ?? DefaultButtonMap;
            _axisMap = axisMap ?? DefaultAxisMap;

            _buttonStates = new Dictionary<Button, bool>();
            _axisStates = new Dictionary<Axis, float>();

            foreach (Button value in Enum.GetValues(typeof(Button)))
                _buttonStates.Add(value, false);
            foreach (Axis value in Enum.GetValues(typeof(Axis)))
                _axisStates.Add(value, 0);

            DeviceId = SDL_JoystickInstanceID(_controller);
        }

        public void ChangeButtonState(SDL_GameControllerButton button, bool state) { if (_buttonMap.ContainsKey(button)) { _buttonStates[_buttonMap[button]] = state; } }
        public void ChangeAxisState(SDL_GameControllerAxis axis, float value) { if (_axisMap.ContainsKey(axis)) { _axisStates[_axisMap[axis]] = value; } }
        public bool ReadButtonState(Button button) => _buttonStates.ContainsKey(button) && _buttonStates[button];
        public float ReadAxisState(Axis axis) => _axisStates.ContainsKey(axis) ? _axisStates[axis] : 0;
        public GameControllerState GetState() => new GameControllerState(_axisStates, _buttonStates);
        
        public static readonly Dictionary<SDL_GameControllerButton, Button> DefaultButtonMap = new Dictionary<SDL_GameControllerButton, Button>
        {
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A, Button.A },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B, Button.B },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X, Button.X },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y, Button.Y },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START, Button.START },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK, Button.BACK },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER, Button.LEFTSHOULDER },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER, Button.RIGHTSHOULDER },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK, Button.LEFTSTICK },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK, Button.RIGHTSTICK },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN, Button.DPAD_DOWN },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT, Button.DPAD_LEFT },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT, Button.DPAD_RIGHT },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP, Button.DPAD_UP },
        };

        public static readonly Dictionary<SDL_GameControllerAxis, Axis> DefaultAxisMap = new Dictionary<SDL_GameControllerAxis, Axis>
        {
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX, Axis.RIGHTX },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY, Axis.RIGHTY },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, Axis.LEFTX },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY, Axis.LEFTY },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT, Axis.TRIGGERLEFT },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT, Axis.TRIGGERRIGHT },
        };
    }
}
