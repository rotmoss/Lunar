using System;
using System.Linq;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Lunar
{
    public class KeyboardState : EventArgs
    {
        public readonly Dictionary<SDL_Keycode, bool> RawKeyStates;

        public KeyboardState(Dictionary<SDL_Keycode, bool> rawKeyStates)
        {
            RawKeyStates = rawKeyStates == null ? new Dictionary<SDL_Keycode, bool>() : rawKeyStates;
        }
    }

    public class Keyboard
    {
        private Dictionary<Key, SDL_Keycode> _keyMap;
        private Dictionary<SDL_Keycode, bool> _rawKeyStates;

        public Keyboard(Dictionary<Key, SDL_Keycode> keyMap = null)
        {
            _keyMap = keyMap == null ? DefaultKeyMap : keyMap;
            _rawKeyStates = Enum.GetValues(typeof(SDL_Keycode)).OfType<SDL_Keycode>().ToDictionary(x => x, x => false);
        }

        public void ChangeRawKeyState(SDL_Keycode key, bool state) => _rawKeyStates[key] = state;
        public void ChangeKeyState(Key key, bool state) => _rawKeyStates[_keyMap[key]] = state;
        public bool ReadRawKeyState(SDL_Keycode key) => _rawKeyStates[key];
        public bool ReadKeyState(Key key) => _rawKeyStates[_keyMap[key]];
        public KeyboardState GetState() => new KeyboardState(_rawKeyStates);

        public static readonly Dictionary<Key, SDL_Keycode> DefaultKeyMap = new Dictionary<Key, SDL_Keycode>
        {
            { Key.BUTTON_0, SDL_Keycode.SDLK_DOWN },
            { Key.BUTTON_1, SDL_Keycode.SDLK_UP },
            { Key.BUTTON_2, SDL_Keycode.SDLK_LEFT },
            { Key.BUTTON_3, SDL_Keycode.SDLK_RIGHT },
            { Key.BUTTON_4, SDL_Keycode.SDLK_t },
            { Key.BUTTON_5, SDL_Keycode.SDLK_a },
            { Key.BUTTON_6, SDL_Keycode.SDLK_s },
            { Key.BUTTON_7, SDL_Keycode.SDLK_d },
            { Key.BUTTON_8, SDL_Keycode.SDLK_f },
            { Key.BUTTON_9, SDL_Keycode.SDLK_g },
            { Key.BUTTON_10, SDL_Keycode.SDLK_h },
            { Key.BUTTON_11,  SDL_Keycode.SDLK_q },
            { Key.BUTTON_12, SDL_Keycode.SDLK_w },
            { Key.BUTTON_13,SDL_Keycode.SDLK_e },
            { Key.BUTTON_14, SDL_Keycode.SDLK_r },
            { Key.BUTTON_15, SDL_Keycode.SDLK_KP_SPACE },
            { Key.BUTTON_31, SDL_Keycode.SDLK_ESCAPE }
        };
    }
}
