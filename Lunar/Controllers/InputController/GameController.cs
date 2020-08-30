using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Lunar
{
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

        public GameControllerState OnButtonStateChange(SDL_GameControllerButton button, SDL_EventType type)
        {
            if (!_buttonMap.ContainsKey(button)) return null;

            if (type == SDL_EventType.SDL_JOYBUTTONDOWN) {
                _buttonStates[_buttonMap[button]] = true;
                return ToGameControllerState();
            }
            else if (type == SDL_EventType.SDL_JOYBUTTONUP) {
                _buttonStates[_buttonMap[button]] = false;
                return ToGameControllerState();
            }
            return null;
        }

        public GameControllerState OnAxisStateChange(SDL_GameControllerAxis axis, float value)
        {
            if (!_axisMap.ContainsKey(axis)) return null;

            _axisStates[_axisMap[axis]] = value;
            return ToGameControllerState();
        }

        public GameControllerState ToGameControllerState()
        {
            GameControllerState e = new GameControllerState();
            e.DeviceId = DeviceId;
            e.ButtonStates = _buttonStates;
            e.AxisStates = _axisStates;
            return e;
        }

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
