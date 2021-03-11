using System;

namespace VoiceChanger.FormatParser
{
    public class AudioContainer
    {
        public AudioContainer(float duration, int sampleRate, int[] data)
        {
            Duration = duration;
            SampleRate = sampleRate;
            Data = data;

            var sampleRateCalculated = data.Length / duration;
            if (sampleRateCalculated != sampleRate)
            {
                throw new Exception();
            }
        }

        public float Duration { get; private set; }

        /// <summary>
        /// Frequency.
        /// </summary>
        public int SampleRate { get; private set; }

        public int[] Data { get; private set; }

        public int ValuesCount => Data.Length;
    }
}
