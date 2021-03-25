using System.Collections.Generic;

namespace VoiceChanger.Utils
{
    public class SampleGeneratorSettings
    {
        public int SampleRate { get; set; }

        public List<SignalGenerationData> Datas { get; set; }
    }

    public class SignalGenerationData
    {
        public int Frequency { get; set; }
        public float TimeStart { get; set; }
        public float Duration { get; set; }
    }
}
