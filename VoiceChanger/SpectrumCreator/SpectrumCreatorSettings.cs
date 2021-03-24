using VoiceChanger.FormatParser;

namespace VoiceChanger.SpectrumCreator
{
    public class SpectrumCreatorSettings
    {
        public SpectrumCreatorSettings(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer;
            StartSignalIndex = 0;
            EndSignalIndex = audioContainer.SamplesCount;
        }

        public int SpectrumMinFrequency { get; set; }
        public int SpectrumMaxFrequency { get; set; }
        public int SpectrumFrequencyStep { get; set; }
        public int StartSignalIndex { get; set; }
        public int EndSignalIndex { get; set; }
        public AudioContainer AudioContainer { get; set; }

    }
}
