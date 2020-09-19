using OpenGL;
using SDL2;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Lunar
{
    partial class GraphicsController
    {
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

        internal uint CreateText(string file, string text, int size, Color color, uint wrapped, out int w, out int h)
        {
            if (!LoadText(file, text, size, color.ToSDL_Color(), wrapped, out IntPtr surface))
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
            Gl.BindTexture(TextureTarget.Texture2d, 0);

            return texture;
        }

        public void SetCurrentTexture(uint id, int index)
        {
            GraphicsObject graphicsObject = _graphicsObjects[id];
            if (graphicsObject.Textures.Length > index) graphicsObject.SelectedTexture = graphicsObject.Textures[index];
        }

        private bool LoadSurface(string file, out IntPtr value)
        {
            try { value = SDL_image.IMG_Load(FileManager.FindFile(file, "Textures")); }
            catch { Console.WriteLine("Couldn't load texture " + file); value = IntPtr.Zero; }
            return value != IntPtr.Zero;
        }

        private bool LoadText(string fontFile, string text, int size, SDL_Color color, uint wrapped, out IntPtr value)
        {
            IntPtr font;
            try { font = SDL_ttf.TTF_OpenFont(FileManager.FindFile(fontFile, "Fonts"), size); }
            catch { Console.WriteLine("Couldn't load font " + fontFile); value = IntPtr.Zero; return false; }

            value = SDL_ttf.TTF_RenderUTF8_Blended_Wrapped(font, text, color, wrapped);            
            return value != IntPtr.Zero;
        }

        private bool GetGLPixelFormat(uint format, out PixelFormat result)
        {
            if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_ABGR8888") { result = PixelFormat.Rgba; return true; }
            if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_ARGB8888") { result = PixelFormat.Bgra; return true; } 
            if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_RGB24") { result = PixelFormat.Rgb; return true; }

            Console.WriteLine("Pixelformat " + SDL_GetPixelFormatName(format) + " is not recognized: "); 
            result = PixelFormat.Rgb; 
            return false;
        }
    }
}
