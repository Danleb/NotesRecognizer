using System;
using System.Collections.Generic;
using System.Linq;
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
            var noteContainer = new NoteContainer();
            const int harmonicsCount = 3;

            foreach (var stringNumber in stringsToAnalyze)
            {
                var stringFrequencies = GuitarTuningNotesCreator.GetStringFrequenciesRange(stringNumber, tonesCount);
                var frequenciesToAnalyze = new List<float>(stringFrequencies);

                for (int i = 0; i < harmonicsCount; i++)
                {
                    var multiplier = MathF.Pow(2, i + 1);
                    frequenciesToAnalyze.AddRange(stringFrequencies.Select(v => v * multiplier));
                }

                foreach (var frequency in stringFrequencies)
                {

                }


            }
            var frequencies = GuitarTuningNotesCreator.GetStringsFrequencies(6);
            var fft = new FastFourierTransformCPU(AudioContainer.Samples);
            var scalogramCreator = new ScalogramCreator(AudioContainer, fft);
            var scalogramContainer = scalogramCreator.CreateScalogram(frequencies);

            foreach (var scalogram in scalogramContainer.Scalograms)
            {
                var localMaximumIndexes = new List<int>();

                if (scalogram.Value[0] > scalogram.Value[1])
                {
                    localMaximumIndexes.Add(0);
                }
                if (scalogram.Value[^1] > scalogram.Value[^2])
                {
                    localMaximumIndexes.Add(0);
                }

                for (int i = 1; i < scalogram.Value.Length - 1; i++)
                {
                    if (scalogram.Value[i] > scalogram.Value[i - 1] && scalogram.Value[i] > scalogram.Value[i + 1])
                    {
                        localMaximumIndexes.Add(i);
                    }
                }


            }

            return noteContainer;
        }
    }
}
