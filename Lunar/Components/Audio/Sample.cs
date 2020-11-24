using System;
using Lunar.Math;
using OpenGL;
using SDL2;

namespace Lunar.Audio
{
    public class Sample : Component<Sample>
    {
        internal IntPtr _chunk;
        internal int _channel;

        public float PanStrength { get => _panStrength; set => _panStrength = value < 0 ? 0 : value > 1 ? 1 : value; }
        private float _panStrength;

        public float FalloffStrength { get => _falloffStrength; set => _falloffStrength = value < 0 ? 0 : value > 1 ? 1 : value; }
        private float _falloffStrength;

        public bool Playing { get => _playing; }
        internal bool _playing;

        public Sample(string file, float falloffStrength = 0.6f, float panStrength = 0.4f) : base()
        {
            _falloffStrength = falloffStrength;
            _panStrength = panStrength;
            Mixer.LoadWAV(file, out _chunk);
        }

        public void Play(int loops)
        {
            int channel = Mixer.GetOpenChannel();
            if (channel < 0) return;

           Mixer.AllocateChannel(channel);
            _channel = channel;

            _playing = true;
            SDL_mixer.Mix_PlayChannel(_channel, _chunk, loops);
            SDL_mixer.Mix_ChannelFinished(Deactivate);
            return;
        }

        public void Deactivate(int channel)
        {
            Mixer.DeallocateChannel(_channel);
            _playing = false;
            _channel = -1;
        }

        public void SetPosition(Vertex2f position)
        {
            if (_channel > -1) {
                byte distance = FormatDistance(position.Length(), FalloffStrength);
                SDL_mixer.Mix_SetPosition(_channel, FormatAngle(position.AngleDegrees(), distance, PanStrength), distance);
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

        public override void DisposeChild()
        {
            if (_chunk != IntPtr.Zero) { SDL_mixer.Mix_FreeChunk(_chunk); }
        }
    }
}
