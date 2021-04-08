using System.Collections.Generic;
using System.Numerics;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;

namespace VoiceChanger.Scalogram
{
    public class ScalogramCreator
    {
        private readonly IFourierTransform _fourierTransform;
        private readonly Complex[] _signalFT;

        public ScalogramCreator(AudioContainer audioContainer, IFourierTransform fourierTransform)
        {
            AudioContainer = audioContainer;
            _fourierTransform = fourierTransform;
            _signalFT = _fourierTransform.CreateTransformZeroPadded();
        }

        public AudioContainer AudioContainer { get; }

        public ScalogramContainer CreateScalogram(List<float> frequenciesToAnalyze)
        {
            var container = new ScalogramContainer(_signalFT.Length, frequenciesToAnalyze.Count);

            for (int frequencyIndex = 0; frequencyIndex < frequenciesToAnalyze.Count; frequencyIndex++)
            {
                var frequency = frequenciesToAnalyze[frequencyIndex];
                var sigma = 6;
                var cyclesCount = 5;
                var scalogram = WaveletTransformCPU.CreateScalogram(frequency, _signalFT, AudioContainer.SampleRate, cyclesCount, sigma);
                container.SetFrequencyData(frequencyIndex, scalogram);
            }

            return container;
        }
    }
}
