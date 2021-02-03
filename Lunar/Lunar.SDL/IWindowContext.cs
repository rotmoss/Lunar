using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.SDL
{
    public interface IWindowContext : IDisposable
    {
        public void SetSize(ViewportSize size);
        public ViewportSize GetSize();
        public void SwapBuffer();
        public void ToggleFullscreen();
    }

    public struct ViewportSize
    {
        public int X;
        public int Y;
        public int W;
        public int H;
    }
}
