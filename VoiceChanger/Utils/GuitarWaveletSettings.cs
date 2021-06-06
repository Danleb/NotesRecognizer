using System.Numerics;

namespace VoiceChanger.Utils
{
    public class GuitarWaveletSettings : WaveletTransformSettings
    {
        public float Coefficient { get; set; }
        public float Bias { get; set; }
        public float Duration { get; set; }

        public override WaveletType WaveletType => WaveletType.Guitar;

        public override Complex[] CreateWavelet(float frequency, int sampleRate, int pointsCount)
        {
            var wavelet = GuitarWaveletCreator.CreateWavelet(frequency, sampleRate, pointsCount, this);
            return wavelet;
        }
    }
}
