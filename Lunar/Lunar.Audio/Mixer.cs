using System;
using Lunar.IO;
using SDL2;

namespace Lunar.Audio
{
    public class Mixer
    {
        static byte NUM_CHANNELS = 2;
        static (int, bool)[] _channels;

        public static void Init()
        {
            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
            SDL_mixer.Mix_AllocateChannels(NUM_CHANNELS);

            _channels = new (int, bool)[NUM_CHANNELS];
            for (byte i = 0; i < NUM_CHANNELS; i++)
                _channels[i] = new(1, false);
        }

        public static bool LoadWAV(string file, out IntPtr sample)
        {
            sample = IntPtr.Zero;
            try {  sample = SDL_mixer.Mix_LoadWAV(FileManager.FindFile(file, "Samples")); }
            catch {  Console.WriteLine("Could not load sample: \"" + file + "\""); }
            return sample != IntPtr.Zero;
        }

        public static int GetOpenChannel()
        {
            for (int i = 0; i < _channels.Length; i++)
                if (_channels[i].Item2 == false) return i;
            return -1;
        }

        public static void AllocateChannel(int channel)
        {
            if (channel > 0 && channel < _channels.Length)
                _channels[channel].Item2 = true;
        }

        public static void DeallocateChannel(int channel)
        {
            if (channel > 0 && channel < _channels.Length)
                _channels[channel].Item2 = false;
        }
        public static void Dispose() => SDL_mixer.Mix_CloseAudio();
    }
}
