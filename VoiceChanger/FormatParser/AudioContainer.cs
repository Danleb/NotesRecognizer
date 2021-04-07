using System;
using System.Linq;

namespace VoiceChanger.FormatParser
{
    public class AudioContainer
    {
        public AudioContainer(float[] samples, int sampleRate)
        {
            throw new NotImplementedException();
        }

        public AudioContainer(float duration, int sampleRate, float[] samples)
        {
            Duration = duration;
            SampleRate = sampleRate;
            Samples = samples;

            var sampleRateCalculated = samples.Length / duration;
            if (sampleRateCalculated != sampleRate)
            {
                throw new Exception();
            }

            Min = Samples.Min();
            Max = Samples.Max();
        }

        public float Duration { get; private set; }

        /// <summary>
        /// Frequency.
        /// </summary>
        public int SampleRate { get; private set; }

        public float[] Samples { get; private set; }

        public int SamplesCount => Samples.Length;

        public float Min { get; private set; }
        public float Max { get; private set; }

        public void Normalize()
        {
            Samples = Samples.Select(v => v / Max).ToArray();
            Min = Samples.Min();
            Max = Samples.Max();
        }
    }
}
