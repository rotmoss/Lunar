using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lunar.IO;
using SDL2;
using Lunar.Math;
using OpenGL;

namespace Lunar.Audio
{
    public static class Mixer
    {
        static byte NUM_CHANNELS = 2;
        static List<Sample> _samples = new List<Sample>();
        static (int, int)[] _channels;

        public static void Init()
        {
            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
            SDL_mixer.Mix_AllocateChannels(NUM_CHANNELS);

            _channels = new (int, int)[NUM_CHANNELS];
            for (byte i = 0; i < NUM_CHANNELS; i++)
                _channels[i] = new(1, -1);
        }

        public static bool LoadWAV(string file, out IntPtr sample)
        {
            sample = IntPtr.Zero;
            try {  sample = SDL_mixer.Mix_LoadWAV(FileManager.FindFile(file, "Samples")); }
            catch {  Console.WriteLine("Could not load sample: \"" + file + "\""); }
            return sample != IntPtr.Zero;
        }

        
        public static void SetPosition(this Sample sample, Vertex2f position)
        {
            int index = _samples.FindIndex(x => x == sample);
            for (int i = 0; i < _channels.Length; i++)
            {
                if (_channels[i].Item2 == index) {
                    byte distance = FormatDistance(position.Length(), sample.FalloffStrength);
                    short angle = FormatAngle(position.AngleDegrees(), distance, sample.PanStrength);
                    SDL_mixer.Mix_SetPosition(_channels[i].Item1, angle, distance);
                    return;
                }
            }
        }

        private static byte FormatDistance(float distance, float falloff)
        {
            distance = MathF.Log((distance / 4f) + 1) / (falloff / 10f);
            if (distance > 255) return 255;
            return (byte)distance;
        }

        private static short FormatAngle(float angle, byte distance, float panStrength)
        {
            if (angle > 90) { angle = 90 + (90 - angle); }
            else if (angle < -90) { angle = -90 + (-90 - angle); }

            float pan = panStrength * (distance / 120f);
            return (short)(180 + (angle * pan));
        }
        public static void Play(this Sample sample, int loops)
        {
            int index = _samples.FindIndex(x => x == sample);
            for (int i = 0; i < _channels.Length; i++)
            {
                if (_channels[i].Item2 < 0)
                {
                    sample._playing = true;
                    _channels[i].Item2 = index;
                    SDL_mixer.Mix_PlayChannel(_channels[i].Item1, sample.Chunk, loops);
                    SDL_mixer.Mix_ChannelFinished(Deactivate);
                    return;
                }
            }
        }

        public static void Deactivate(int channel) { _samples[_channels[channel].Item2]._playing = false; _channels[channel].Item2 = -1; }
        public static void AddSample(Sample value) => _samples.Add(value);
        public static void RemoveSample(Sample value) => _samples.Remove(value);
        public static void Dispose()
        {
            for (int i = 0; i < _samples.Count; i++)
                SDL_mixer.Mix_FreeChunk(_samples[i].Chunk);

            _samples.Clear();
            SDL_mixer.Mix_CloseAudio();
        }
    }
}
