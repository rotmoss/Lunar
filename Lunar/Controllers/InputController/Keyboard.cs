using System;
using System.Linq;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Lunar
{
    public class Keyboard
    {
        private Dictionary<SDL_Keycode, Key> _keyMap;
        private Dictionary<Key, bool> _keyStates;

        public Keyboard(Dictionary<SDL_Keycode, Key> keyMap = null)
        {
            _keyMap = keyMap == null ? DefaultKeyMap : keyMap;
            _keyStates = Enum.GetValues(typeof(Key)).OfType<Key>().ToDictionary(x => x, x => false);
        }

        public void ChangeRawKeyState(SDL_Keycode key, bool state)
        {
            if (!_keyMap.ContainsKey(key)) return;
            _keyStates[_keyMap[key]] = state;
        }

        public void ChangeKeyState(Key key, bool state)
        {
            _keyStates[key] = state;
        }

        public bool ReadKeyState(Key key)
        {
            return _keyStates[key];
        }

        public bool ReadRawKeyState(SDL_Keycode key)
        {
            if (!_keyMap.ContainsKey(key)) return false;
            return _keyStates[_keyMap[key]];
        }

        public static readonly Dictionary<SDL_Keycode, Key> DefaultKeyMap = new Dictionary<SDL_Keycode, Key>
        {
            { SDL_Keycode.SDLK_DOWN, Key.BUTTON_0 },
            { SDL_Keycode.SDLK_UP, Key.BUTTON_1 },
            { SDL_Keycode.SDLK_LEFT, Key.BUTTON_2 },
            { SDL_Keycode.SDLK_RIGHT, Key.BUTTON_3 },
            { SDL_Keycode.SDLK_q, Key.BUTTON_11 },
            { SDL_Keycode.SDLK_w, Key.BUTTON_12 },
            { SDL_Keycode.SDLK_e, Key.BUTTON_13 },
            { SDL_Keycode.SDLK_r, Key.BUTTON_14 },
            { SDL_Keycode.SDLK_t, Key.BUTTON_4 },
            { SDL_Keycode.SDLK_a, Key.BUTTON_5 },
            { SDL_Keycode.SDLK_s, Key.BUTTON_6 },
            { SDL_Keycode.SDLK_d, Key.BUTTON_7 },
            { SDL_Keycode.SDLK_f, Key.BUTTON_8 },
            { SDL_Keycode.SDLK_g, Key.BUTTON_9 },
            { SDL_Keycode.SDLK_h, Key.BUTTON_10 },
            { SDL_Keycode.SDLK_KP_SPACE, Key.BUTTON_15 },
            { SDL_Keycode.SDLK_ESCAPE, Key.BUTTON_31 }
        };
    }
}
