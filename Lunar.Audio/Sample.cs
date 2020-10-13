using System;
using System.Collections.Generic;
using System.Numerics;
using Lunar.IO;
using Lunar.Math;
using Microsoft.VisualBasic.CompilerServices;
using SDL2;

namespace Lunar.Audio
{
    public class Sample
    {
        static int NUM_CHANNELS = 16;
        static HashSet<int> channels = new HashSet<int>();
        IntPtr sample;
        int channel;
        public Sample(string file)
        {
            LoadWAV(file, out sample);
            for(int i = 0; i < NUM_CHANNELS; i++)
            {
                if(!channels.Contains(i)) { channel = i; channels.Add(i); return; }
            }
            channel = 0;
        }

        public static void Init()
        {
            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
            SDL_mixer.Mix_AllocateChannels(NUM_CHANNELS);
        }

        public void SetPosition(Vector2 position)
        {
            SDL_mixer.Mix_SetPosition(channel, (short)(position.AngleDegrees() + 180), (byte)position.Length());
            Console.WriteLine(position.AngleDegrees());
        }

        public void PlaySample(int loops)
        {
            SDL_mixer.Mix_PlayChannel(channel, sample, loops);
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
