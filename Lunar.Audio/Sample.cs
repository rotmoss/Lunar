using System;
using Lunar.IO;
using SDL2;

namespace Lunar.Audio
{
    public class Sample
    {
        IntPtr sample;
        public Sample(string file)
        {
            LoadWAV(file, out sample);
        }

        private bool LoadWAV(string file, out IntPtr sample)
        {
            sample = IntPtr.Zero;
            try
            {
                sample = SDL_mixer.Mix_LoadWAV(FileManager.FindFile(file, "Samples"));
            } catch
            {
                Console.WriteLine("Could not load sample: \"" + file + "\"");
            }
            return sample != IntPtr.Zero;
        }
    }
}
