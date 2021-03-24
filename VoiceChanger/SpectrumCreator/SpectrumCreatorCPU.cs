using System;
using System.Collections.Generic;
using VoiceChanger.FormatParser;

namespace VoiceChanger.SpectrumCreator
{
    public class SpectrumCreatorCPU : SpectrumCreator
    {
        public SpectrumCreatorCPU(AudioContainer audioContainer) : base(audioContainer)
        {

        }

        public override SpectrumContainer CreateSpectrum(SpectrumCreatorSettings settings)
        {
            //settings.AudioContainer.SampleRate
            var slices = new List<SpectrumSlice>();

            for (var frequency = settings.SpectrumMinFrequency; frequency < settings.SpectrumMaxFrequency; frequency += settings.SpectrumFrequencyStep)
            {
                var scale = frequency;

                for (int i = 0; i < 100; i++)
                {

                }
            }

            Console.WriteLine("!!!");
            var container = new SpectrumContainer(slices);
            return container;
        }
    }
}
