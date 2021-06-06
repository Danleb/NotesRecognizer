using System.Numerics;
using VoiceChanger.SpectrumCreator;

namespace VoiceChanger.Utils
{
    public class MorletWaveletSettings : WaveletTransformSettings
    {
        public float CyclesCount { get; set; }

        public override WaveletType WaveletType => WaveletType.Morlet;

        public override Complex[] CreateWavelet(float frequency, int sampleRate, int pointsCount)
        {
            var wavelet = WaveletTransformCPU.GenerateMorletWavelet(frequency, sampleRate, CyclesCount, pointsCount, 3);
            return wavelet;
        }
    }
}
