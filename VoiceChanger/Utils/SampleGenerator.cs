using System;
using VoiceChanger.FormatParser;

namespace VoiceChanger.Utils
{
    public static class SampleGenerator
    {
        private const float TwoPI = 2.0f * MathF.PI;

        public static AudioContainer GenerateSineSignal(float frequency, float duration, int sampleRate)
        {
            var samplesCount = (int)Math.Round(sampleRate * duration);
            return GenerateSineSignal(frequency, samplesCount, sampleRate);
        }

        public static AudioContainer GenerateSineSignal(float frequency, int samplesCount, int sampleRate)
        {
            var samplePeriod = 1.0f / sampleRate;
            var samples = new float[samplesCount];
            var angularFrequency = TwoPI * frequency;

            for (var sampleIndex = 0; sampleIndex < samplesCount; sampleIndex++)
            {
                var time = sampleIndex * samplePeriod;
                var value = MathF.Sin(angularFrequency * time);
                samples[sampleIndex] = value;
            }

            var recalculatedDuration = samplesCount * samplePeriod;
            var container = new AudioContainer(recalculatedDuration, sampleRate, samples);
            return container;
        }
    }
}
