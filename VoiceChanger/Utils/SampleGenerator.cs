using System;
using System.Collections.Generic;
using System.Linq;
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

        public static AudioContainer GenerateSample(SampleGeneratorSettings settings)
        {
            var container = GenerateSineSignal(settings.Datas[0].Frequency, settings.Datas[0].Duration, settings.SampleRate);

            const float Epsilon = 1e-3f;

            var frequencyChanges = new SortedSet<float>();
            foreach (var v in settings.Datas)
            {
                frequencyChanges.Add(v.TimeStart);
                frequencyChanges.Add(v.TimeStart + v.Duration);
            }
            //var timeAnchors = settings.Datas.Select(v => v.TimeStart);

            var toDelete = new List<float>();
            var previous = frequencyChanges.First();
            foreach (var time in frequencyChanges)
            {

            }

            foreach (var time in toDelete)
            {
                frequencyChanges.Remove(time);
            }

            foreach(var time in frequencyChanges)
            {

            }

            return container;
        }
    }
}
