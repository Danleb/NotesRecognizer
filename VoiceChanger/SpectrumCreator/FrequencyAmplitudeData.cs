namespace VoiceChanger.SpectrumCreator
{
    public class FrequencyAmplitudeData
    {
        public FrequencyAmplitudeData()
        {

        }

        public FrequencyAmplitudeData(int frequency, float amplitude)
        {
            Frequency = frequency;
            Amplitude = amplitude;
        }

        public int Frequency { get; set; }
        public float Amplitude { get; set; }

        public override string ToString()
        {
            return $"Frequency={Frequency};Amplitude={Amplitude}";
        }
    }
}
