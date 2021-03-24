using System.Collections.Generic;
using System.Linq;

namespace VoiceChanger.SpectrumCreator
{
    public class SpectrumSlice
    {
        public float MinFrequency { get; }
        public float MaxFrequency { get; }
        public float MaxAmplitude { get; }
        public float MinAmplitude => 0.0f;
        public IReadOnlyList<FrequencyAmplitudeData> Datas { get; }

        public SpectrumSlice()
        {

        }

        public SpectrumSlice(List<FrequencyAmplitudeData> datas)
        {
            Datas = datas.AsReadOnly();
            MaxAmplitude = Datas.Max(v => v.Amplitude);
            MinFrequency = Datas.Min(v => v.Frequency);
            MaxFrequency = Datas.Max(v => v.Frequency);
        }

        public float GetAmplitudeForFrequency(int frequency)
        {
            return 0;
        }
    }
}
