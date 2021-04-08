using System;

namespace VoiceChanger.Scalogram
{
    public class ScalogramContainer
    {
        private readonly int _signalsCount;

        public ScalogramContainer(int signalsCount, int frequenciesCount)
        {
            ScalogramValues = new float[signalsCount * frequenciesCount];
            _signalsCount = signalsCount;
        }

        public float[] ScalogramValues { get; }

        public void SetFrequencyData(int frequencyIndex, float[] scalogramRowValues)
        {
            if (scalogramRowValues.Length != _signalsCount)
            {
                throw new Exception("scalogramRowValues.Length doesn't equal _signalsCount");
            }

            var indexStart = frequencyIndex * _signalsCount;
            Array.Copy(scalogramRowValues, 0, ScalogramValues, indexStart, scalogramRowValues.Length);
        }
    }
}
