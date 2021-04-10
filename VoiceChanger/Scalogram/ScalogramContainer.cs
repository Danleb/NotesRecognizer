using System;
using System.Linq;

namespace VoiceChanger.Scalogram
{
    public class ScalogramContainer
    {
        public ScalogramContainer(int signalsCount, int frequenciesCount)
        {
            SignalsCount = signalsCount;
            FrequenciesCount = frequenciesCount;
            ScalogramValues = new float[signalsCount * frequenciesCount];
        }

        public float[] ScalogramValues { get; }
        public int FrequenciesCount { get; }
        public int SignalsCount { get; }

        public void SetFrequencyData(int frequencyIndex, float[] scalogramRowValues)
        {
            if (scalogramRowValues.Length != SignalsCount)
            {
                throw new Exception("scalogramRowValues.Length doesn't equal _signalsCount");
            }

            var indexStart = frequencyIndex * SignalsCount;
            Array.Copy(scalogramRowValues, 0, ScalogramValues, indexStart, scalogramRowValues.Length);
        }

        public void Normalize()
        {
            var max = ScalogramValues.Max();
            for (int i = 0; i < ScalogramValues.Length; i++)
            {
                ScalogramValues[i] /= max;
            }
        }
    }
}
