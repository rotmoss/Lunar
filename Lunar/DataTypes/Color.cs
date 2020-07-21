using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public struct Color
    {
        public byte r, g, b, a;

        public Color(byte r = 0, byte g = 0, byte b = 0, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public SDL2.SDL.SDL_Color ToSDL_Color() => new SDL2.SDL.SDL_Color { r = r, g = g, b = b, a = a };
    }
}
