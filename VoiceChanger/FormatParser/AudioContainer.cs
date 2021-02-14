namespace VoiceChanger.FormatParser
{
    public class AudioContainer
    {
        public AudioContainer(float duration, int sampleRate, int[] data)
        {
            Duration = duration;
            SampleRate = sampleRate;
            Data = data;
        }

        public float Duration { get; private set; }
        public int SampleRate { get; private set; }

        public int[] Data { get; private set; }
    }
}
