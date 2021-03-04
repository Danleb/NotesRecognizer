using VoiceChanger.SpectrumCreator;

namespace VoiceChanger.NoteRecognizer
{
    public class NoteRecognizer
    {
        public SpectrumContainer SpectrumContainer { get; }

        public NoteRecognizer(SpectrumContainer spectrumContainer)
        {
            SpectrumContainer = spectrumContainer;
        }


    }
}
