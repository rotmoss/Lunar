using System;
using System.Collections.Generic;
using System.Numerics;
using Lunar.IO;
using Lunar.Math;
using SDL2;

namespace Lunar.Audio
{
    public class Sample
    {
        static int NUM_CHANNELS = 16;
        static HashSet<int> Channels = new HashSet<int>();
        static List<Sample> Samples = new List<Sample>();

        public float PanStrength { get => _panStrength; set => _panStrength = value < 0 ? 0 : value > 1 ? 1 : value; }
        float _panStrength;

        public float FalloffStrength { get => _falloffStrength; set => _falloffStrength = value < 0 ? 0 : value > 1 ? 1 : value; }
        float _falloffStrength;

        IntPtr _chunk;
        int _channel;
        public Sample(string file)
        {
            _falloffStrength = 0.6f;
            _panStrength = 0.4f;
            LoadWAV(file, out _chunk);
            for(int i = 0; i < NUM_CHANNELS; i++)
            {
                if(!Channels.Contains(i)) { _channel = i; Channels.Add(i); return; }
            }
            _channel = 0;

            Samples.Add(this);
        }

        public static void Init()
        {
            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
            SDL_mixer.Mix_AllocateChannels(NUM_CHANNELS);
        }

        public void SetPosition(Vector2 position)
        {
            byte distance = FormatDistance(position.Length());
            short angle = FormatAngle(position.AngleDegrees(), distance);
            SDL_mixer.Mix_SetPosition(_channel, angle, distance);
        }

        public byte FormatDistance(float distance)
        {
            distance = MathF.Log((distance / 4f) + 1) / (_falloffStrength / 10f);
            if (distance > 255) return 255;
            return (byte)distance;
        }

        public short FormatAngle(float angle, byte distance)
        {
            if (angle > 90) { angle = 90 + (90 - angle); }
            else if (angle < -90) { angle = -90 + (-90 - angle); }

            float pan = _panStrength * (distance / 120f);
            return (short)(180 + (angle * pan));
        }

        public void PlaySample(int loops)
        {
            SDL_mixer.Mix_PlayChannel(_channel, _chunk, loops);
        }

        private static bool LoadWAV(string file, out IntPtr sample)
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

        public void Dispose()
        {
            if (_chunk != IntPtr.Zero) { SDL_mixer.Mix_FreeChunk(_chunk); }
            Samples.Remove(this);
        }

        public static void DisposeAll()
        {
            foreach(Sample sample in Samples)
            {
                if (sample._chunk != IntPtr.Zero) { SDL_mixer.Mix_FreeChunk(sample._chunk); }
                SDL_mixer.Mix_CloseAudio();
            }
        }
    }
}
