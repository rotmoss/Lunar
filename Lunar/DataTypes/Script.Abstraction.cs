using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public abstract partial class Script
    {
        protected bool KeyDown(Key key) => InputController.KeyDown(key);
        protected bool KeyDown(SDL2.SDL.SDL_Keycode key) => InputController.KeyDown(key);
    }
}
