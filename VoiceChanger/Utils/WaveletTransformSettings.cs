using System.Numerics;

namespace VoiceChanger.Utils
{
    public abstract class WaveletTransformSettings
    {
        public abstract WaveletType WaveletType { get; }

        public abstract Complex[] CreateWavelet(float frequency, int sampleRate, int pointsCount);
    }
}
