using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VoiceChanger.NoteRecognizer;

namespace VoiceChanger.Scalogram
{
    public class ScalogramContainer
    {
        private readonly List<FrequencyScalogram> _scalograms = new();

        public ScalogramContainer(int signalsCount)
        {
            SignalsCount = signalsCount;
        }

        public int SignalsCount { get; }
        public int FrequenciesCount => _scalograms.Count;
        public IEnumerable<float> Frequencies => _scalograms.Select(v => v.FrequencyData.Frequency);
        public IReadOnlyList<FrequencyScalogram> Scalograms => _scalograms;
        public ConcurrentDictionary<FrequencyData, float[]> Plots = new();

        public void SetFrequencyData(FrequencyData frequency, float[] scalogramValues)
        {
            if (scalogramValues.Length != SignalsCount)
            {
                throw new Exception("scalogramRowValues.Length doesn't equal _signalsCount");
            }

            _scalograms.Add(new(frequency, scalogramValues));
        }

        public float[] GetFrequencyScalogram(float frequency)
        {
            return _scalograms.Single(v => v.FrequencyData == frequency).Values;
        }

        public void NormalizeGlobal()
        {
            var max = float.MinValue;
            foreach (var scalogram in _scalograms)
            {
                max = Math.Max(max, scalogram.Values.Max());
            }

            foreach (var scalogram in _scalograms)
            {
                for (int i = 0; i < scalogram.Length; i++)
                {
                    scalogram[i] /= max;
                }
            }
        }

        public void NormalizeEachLocally()
        {
            foreach (var scalogram in _scalograms)
            {
                var max = scalogram[0];
                for (int i = 1; i < scalogram.Length; i++)
                {
                    if (scalogram[i] > max)
                    {
                        max = scalogram[i];
                    }
                }

                for (int i = 0; i < scalogram.Length; i++)
                {
                    scalogram[i] /= max;
                }
            }
        }
    }
}
