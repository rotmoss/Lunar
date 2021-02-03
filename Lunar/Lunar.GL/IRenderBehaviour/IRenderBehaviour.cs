using Lunar.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using Lunar.SDL;

namespace Lunar.GL
{
    public interface IRenderBehaviour : IDisposable
    {
        public static List<string> RenderLayers { get; }
        public static Dictionary<string, IShaderStorageBuffer> ShaderStorage { get; }

        public void AddLayers(params string[] value);
        public void SetShaderStorage(object value);
        public void Render();
        public void SetSize(ViewportSize size);
    }
}
