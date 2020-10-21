using System;
using System.Numerics;
using Lunar.Math;
using SDL2;

namespace Lunar.Audio
{
    public class Sample
    {
        internal IntPtr Chunk;
        public float PanStrength { get => _panStrength; set => _panStrength = value < 0 ? 0 : value > 1 ? 1 : value; }
        private float _panStrength;

        public float FalloffStrength { get => _falloffStrength; set => _falloffStrength = value < 0 ? 0 : value > 1 ? 1 : value; }
        private float _falloffStrength;

        public bool Playing { get => _playing; }
        internal bool _playing;

        public Sample(string file, float falloffStrength = 0.6f, float panStrength = 0.4f)
        {
            _falloffStrength = falloffStrength;
            _panStrength = panStrength;
            Mixer.LoadWAV(file, out Chunk);
            Mixer.AddSample(this);
        }

        public void Dispose()
        {
            if (Chunk != IntPtr.Zero) { SDL_mixer.Mix_FreeChunk(Chunk); }
            Mixer.RemoveSample(this);
        }
    }
}
