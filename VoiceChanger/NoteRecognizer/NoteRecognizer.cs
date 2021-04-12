using System.Collections.Generic;
using VoiceChanger.FormatParser;
using VoiceChanger.Scalogram;
using VoiceChanger.SpectrumCreator;

namespace VoiceChanger.NoteRecognizer
{
    public class NoteRecognizer
    {

        public NoteRecognizer(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer;
        }

        public AudioContainer AudioContainer { get; }
        public ScalogramCreator ScalogramCreator { get; }

        public NoteContainer CreateNoteContainer(List<int> stringsToAnalyze, int tonesCount)
        {
            var frequencies = GuitarTuningNotesCreator.GetStringsFrequencies(6);
            var fft = new FastFourierTransformCPU(AudioContainer.Samples);
            var scalogramCreator = new ScalogramCreator(AudioContainer, fft);
            var scalogramContainer = scalogramCreator.CreateScalogram(frequencies);

            var noteContainer = new NoteContainer();



            return noteContainer;
        }
    }
}
