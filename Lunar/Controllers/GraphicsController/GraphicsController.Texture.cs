using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using static SDL2.SDL;
using System.Runtime.InteropServices;
using OpenGL;

namespace Lunar
{
    partial class GraphicsController
    {
        private Dictionary<uint, List<uint>> _textures;
        private Dictionary<uint, int> _selectedTexture;

        internal uint CreateTexture(string file, out int w, out int h)
        {
            if(!LoadSurface(file, out IntPtr surface))
            { w = 0; h = 0; return 0; }

            SDL_Surface temp = Marshal.PtrToStructure<SDL_Surface>(surface);
            w = temp.w; h = temp.h;

           uint texture = StoreTextureOnGpu(temp);
            SDL_FreeSurface(surface);

            return texture;
        }

        private uint StoreTextureOnGpu(SDL_Surface surface)
        {
            if (!GetGLPixelFormat(Marshal.PtrToStructure<uint>(surface.format), out PixelFormat format)) return 0;
         
            uint texture = Gl.GenTexture();

            Gl.Enable(EnableCap.Texture2d);
            Gl.BindTexture(TextureTarget.Texture2d, texture);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, surface.w, surface.h, 0, format, PixelType.UnsignedByte, surface.pixels);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.NEAREST);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.NEAREST);
            Gl.Disable(EnableCap.Texture2d);

            return texture;
        }

        public void SetCurrentTexture(uint id, int index)
        {
            if(!_selectedTexture.ContainsKey(id)) { return; }

            if (index < _textures.Count) _selectedTexture[id] = index;
            else _selectedTexture[id] = 0;
        }

        private bool LoadSurface(string file, out IntPtr value)
        {
            if(file == null) { value = IntPtr.Zero; return false; }
            try { value = SDL_image.IMG_Load(FileManager.FindFile(file, "Textures")); }
            catch { Console.WriteLine("Couldn't load texture " + file); value = IntPtr.Zero; }
            return value == IntPtr.Zero ? false : true;
        }

        private bool GetGLPixelFormat(uint format, out PixelFormat result)
        {
            if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_ABGR8888") { result = PixelFormat.Rgba; return true; }
            else if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_ARGB8888") { result = PixelFormat.Bgra; return true; }
            else if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_RGB24") { result = PixelFormat.Rgb; return true; }

            Console.WriteLine("Pixelformat " + SDL_GetPixelFormatName(format) + " is not recognized: "); 
            result = PixelFormat.Rgb; 
            return false;
        }

        internal void BindTexture(uint id) { Gl.Enable(EnableCap.Texture2d); Gl.BindTexture(TextureTarget.Texture2d, _textures[id][_selectedTexture[id]]); }
        internal void UnBindTexture() { Gl.BindTexture(TextureTarget.Texture2d, 0); Gl.Disable(EnableCap.Texture2d); }
        internal void DeleteTexture(uint id) => Gl.DeleteTextures(_textures[id][_selectedTexture[id]]);
    }
}
