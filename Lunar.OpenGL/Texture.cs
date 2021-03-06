using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Silk.NET.OpenGL;

namespace Lunar.OpenGL
{
    public class Texture
    {
        public uint Id { get => _id; }
        private uint _id;

        public int Width { get => _width; }
        private int _width;

        public int Height { get => _height; }
        private int _height;

        private static Dictionary<string, Texture> _textures = new Dictionary<string, Texture> ();

        private Texture(uint id, int h, int w)
        {
            _id = id;
            _width = w;
            _height = h;
        }

        public static unsafe uint CreateTexture(Image<Rgba32> img) 
        {
            uint id = Engine.GL.GenTexture();
            Engine.GL.BindTexture(TextureTarget.Texture2D, id);

            fixed (void* data = &MemoryMarshal.GetReference(img.GetPixelRowSpan(0)))
            {
                Engine.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                img.Dispose();
            }

            Engine.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (float)GLEnum.Nearest);
            Engine.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (float)GLEnum.Nearest);      

            return id;
        }

        public static unsafe void Resize(uint id, int w, int h)
        {
            Engine.GL.BindTexture(TextureTarget.Texture2D, id);
            Engine.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, (uint)w, (uint)h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
        }

        public static unsafe Texture LoadImage(string path)
        {
            if(_textures.ContainsKey(path))
                return _textures[path];

            Image<Rgba32> img = (Image<Rgba32>) Image.Load(Engine.Path + "Textures" + Engine.Seperator + path);
            img.Mutate(x => x.Flip(FlipMode.Vertical));

            Texture texture = new Texture(CreateTexture(img), img.Width, img.Height);
            _textures.Add(path, texture);

            return texture;
        }

        public static void Dispose(string file) 
        {
            Engine.GL.DeleteTexture(_textures[file].Id); 
            _textures.Remove(file);
        }

        public static void Dispose() 
        {
            foreach(Texture texture in _textures.Values) 
                Engine.GL.DeleteTexture(texture.Id); 

            _textures.Clear();
        }

        public override int GetHashCode() => (int)Id;
        public override bool Equals(object obj) => Equals(obj as Texture);
        public bool Equals(Texture obj) => obj != null && obj.Id == Id;
    }
}