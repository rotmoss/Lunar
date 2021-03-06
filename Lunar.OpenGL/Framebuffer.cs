using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Lunar.OpenGL
{
    public struct Framebuffer
    {
        public uint id;
        public Framebuffer(uint[] textures)
        {
            id = Engine.GL.GenFramebuffer();
            Engine.GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);

            for (int i = 0; i < textures.Length; i++) {
                Engine.GL.FramebufferTexture(GLEnum.Framebuffer, (GLEnum)(GLEnum.ColorAttachment0 + i), textures[i], 0);
                Engine.GL.DrawBuffer((GLEnum)(GLEnum.ColorAttachment0 + i));
            }

            Engine.GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            Engine.GL.DeleteFramebuffer(id);
        }
    }
}
