namespace VoiceChanger.SpectrumCreator
{
    public class FrequencyAmplitudeData
    {
        public FrequencyAmplitudeData()
        {

        }

        public FrequencyAmplitudeData(float frequency, float amplitude)
        {
            Frequency = frequency;
            Amplitude = amplitude;
        }

        public float Frequency { get; set; }
        public float Amplitude { get; set; }

        public override string ToString()
        {
            return $"Frequency={Frequency};Amplitude={Amplitude}";
        }
    }
}
