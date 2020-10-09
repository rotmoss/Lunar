using OpenGL;
using SDL2;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Lunar.Graphics
{
    class Texture
    {
        public uint id;
        public string name;
        public int w;
        public int h;

        private static List<Texture> _textures = new List<Texture>();

        public static bool CreateTexture(string file, out int w, out int h, out Texture texture)
        {
            foreach (Texture t in _textures) { if (t.name == file) 
            { texture = t; w = t.w; h = t.w; return true; } }

            texture = new Texture();

            if (!LoadSurface(file, out IntPtr surface))
            { w = 0; h = 0; return false; }

            SDL_Surface temp = Marshal.PtrToStructure<SDL_Surface>(surface);
            texture.w = w = temp.w; texture.h = h = temp.h;

            texture.id = StoreTextureOnGpu(temp);
            SDL_FreeSurface(surface);

            _textures.Add(texture);
            return true;
        }

        public static bool CreateText(string file, string message, int size, uint wrapped, byte r, byte g, byte b, byte a, out int w, out int h, out Texture texture)
        {
            foreach (Texture t in _textures)
            {
                if (t.name == file)
                { texture = t; w = t.w; h = t.w; return true; }
            }

            texture = new Texture();

            if (!LoadText(file, message, size, wrapped, new SDL_Color { r = r, g = g, b = b, a = a }, out IntPtr surface))
            { w = 0; h = 0; return false; }

            SDL_Surface temp = Marshal.PtrToStructure<SDL_Surface>(surface);
            texture.w = w = temp.w; texture.h = h = temp.h;

            texture.id = StoreTextureOnGpu(temp);
            SDL_FreeSurface(surface);

            _textures.Add(texture);
            return true;
        }

        private static uint StoreTextureOnGpu(SDL_Surface surface)
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

        private static bool LoadSurface(string file, out IntPtr value)
        {
            try { value = SDL_image.IMG_Load(IO.FileManager.FindFile(file, "Textures")); }
            catch { Console.WriteLine("Couldn't load texture " + file); value = IntPtr.Zero; }
            return value != IntPtr.Zero;
        }

        private static bool LoadText(string file, string text, int size, uint wrapped, SDL_Color color,  out IntPtr value)
        {
            IntPtr font;
            try { font = SDL_ttf.TTF_OpenFont(IO.FileManager.FindFile(file, "Fonts"), size); }
            catch { Console.WriteLine("Couldn't load font " + file); value = IntPtr.Zero; return false; }

            value = SDL_ttf.TTF_RenderUTF8_Blended_Wrapped(font, text, color, wrapped);
            return value != IntPtr.Zero;
        }

        private static bool GetGLPixelFormat(uint format, out PixelFormat result)
        {
            if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_ABGR8888") { result = PixelFormat.Rgba; return true; }
            if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_ARGB8888") { result = PixelFormat.Bgra; return true; }
            if (SDL_GetPixelFormatName(format) == "SDL_PIXELFORMAT_RGB24") { result = PixelFormat.Rgb; return true; }

            Console.WriteLine("Pixelformat " + SDL_GetPixelFormatName(format) + " is not recognized: ");
            result = PixelFormat.Rgb;
            return false;
        }

        public void Dispose()
        {
            foreach (Texture texture in _textures)
            {
                Gl.DeleteTextures(texture.id);
            }
        }
    }
}
