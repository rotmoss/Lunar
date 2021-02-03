using System;
using System.Collections.Generic;
using OpenGL;

namespace Lunar.GL
{
    public struct Framebuffer
    {
        public uint id;
        public Framebuffer(Texture[] textures)
        {
            id = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, id);

            for (int i = 0; i < textures.Length; i++) {
                Gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, textures[i].id, 0);
                Gl.DrawBuffers((int)FramebufferAttachment.ColorAttachment0 + i);
            }

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            Gl.DeleteFramebuffers(id);
        }
    }
}
