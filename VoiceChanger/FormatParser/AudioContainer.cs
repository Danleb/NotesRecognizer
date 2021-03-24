using System;
using System.Linq;

namespace VoiceChanger.FormatParser
{
    public class AudioContainer
    {
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

            Max = Samples.Max();
            Min = Samples.Min();
        }

        public float Duration { get; private set; }

        /// <summary>
        /// Frequency.
        /// </summary>
        public int SampleRate { get; private set; }

        public float[] Samples { get; private set; }

        public int SamplesCount => Samples.Length;

        public float Max { get; }
        public float Min { get; }
    }
}
