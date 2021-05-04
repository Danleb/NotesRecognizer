using System;
using System.Collections.Generic;
using System.Linq;

namespace VoiceChanger.Scalogram
{
    public class ScalogramContainer
    {
        private Dictionary<float, float[]> _scalogramValues = new();

        public ScalogramContainer(int signalsCount)
        {
            SignalsCount = signalsCount;
        }

        public int SignalsCount { get; }
        public int FrequenciesCount => _scalogramValues.Count;
        public IEnumerable<float> Frequencies => _scalogramValues.Keys;
        public IEnumerable<KeyValuePair<float, float[]>> Scalograms => _scalogramValues;

        public void SetFrequencyData(float frequency, float[] scalogramValues)
        {
            if (scalogramValues.Length != SignalsCount)
            {
                throw new Exception("scalogramRowValues.Length doesn't equal _signalsCount");
            }

            _scalogramValues.Add(frequency, scalogramValues);
        }

        public void Normalize()
        {
            //for (int frequencyIndex = 0; frequencyIndex < FrequenciesCount; frequencyIndex++)
            //{
            //    var max = ScalogramValues[frequencyIndex * SignalsCount];
            //    for (int u = frequencyIndex * SignalsCount; u < (frequencyIndex + 1) * SignalsCount; u++)
            //    {
            //        if (ScalogramValues[u] > max)
            //        {
            //            max = ScalogramValues[u];
            //        }
            //    }

            //    for (int u = frequencyIndex * SignalsCount; u < (frequencyIndex + 1) * SignalsCount; u++)
            //    {
            //        ScalogramValues[u] /= max;
            //    }
            //}

            var max = float.MinValue;
            foreach (var scalogram in _scalogramValues.Values)
            {
                max = Math.Max(max, scalogram.Max());
            }

            foreach (var scalogram in _scalogramValues.Values)
            {
                for (int i = 0; i < scalogram.Length; i++)
                {
                    scalogram[i] /= max;
                }
            }
        }

        public float[] GetFrequencyScalogram(float frequency)
        {
            return _scalogramValues[frequency];
        }
    }
}
