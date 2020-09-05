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

            _buttonMap = buttonMap == null ? DefaultButtonMap : buttonMap;
            _axisMap = axisMap == null ? DefaultAxisMap : axisMap;

            _buttonStates = new Dictionary<Button, bool>();
            _axisStates = new Dictionary<Axis, float>();

            foreach (Button value in Enum.GetValues(typeof(Button)))
                _buttonStates.Add(value, false);
            foreach (Axis value in Enum.GetValues(typeof(Axis)))
                _axisStates.Add(value, 0);

            DeviceId = SDL_JoystickInstanceID(_controller);
        }

        public void ChangeButtonState(SDL_GameControllerButton button, bool state) => _buttonStates[_buttonMap[button]] = _buttonMap.ContainsKey(button) ? state : _buttonStates[_buttonMap[button]];
        public void ChangeAxisState(SDL_GameControllerAxis axis, float value) => _axisStates[_axisMap[axis]] = _axisMap.ContainsKey(axis) ? value : _axisStates[_axisMap[axis]];
        
        public GameControllerState GetState() => new GameControllerState(_axisStates, _buttonStates);
        
        public static readonly Dictionary<SDL_GameControllerButton, Button> DefaultButtonMap = new Dictionary<SDL_GameControllerButton, Button>
        {
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A, Button.BUTTON_0 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B, Button.BUTTON_1 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X, Button.BUTTON_2 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y, Button.BUTTON_3 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START, Button.BUTTON_4 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK, Button.BUTTON_5 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER, Button.BUTTON_6 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER, Button.BUTTON_7 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK, Button.BUTTON_8 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK, Button.BUTTON_9 },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN, Button.DPAD_DOWN },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT, Button.DPAD_LEFT },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT, Button.DPAD_RIGHT },
            { SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP, Button.DPAD_UP },
        };

        public static readonly Dictionary<SDL_GameControllerAxis, Axis> DefaultAxisMap = new Dictionary<SDL_GameControllerAxis, Axis>
        {
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX, Axis.AXIS_0 },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY, Axis.AXIS_1 },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, Axis.AXIS_2  },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY, Axis.AXIS_3 },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT, Axis.AXIS_4  },
            { SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT, Axis.AXIS_5  },
        };
    }
}
