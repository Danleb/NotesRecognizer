using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Numerics;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;

namespace VoiceChanger.Scalogram
{
    public class ScalogramCreator
    {
        private readonly ILogger _logger;
        private readonly Complex[] _signalFT;
        private readonly IFourierTransform _fourierTransform;
        private int _frequenciesCalculated;
        private List<float> _frequenciesToAnalyze;

        public ScalogramCreator(AudioContainer audioContainer, IFourierTransform fourierTransform)
        {
            AudioContainer = audioContainer;
            _fourierTransform = fourierTransform;
            _signalFT = _fourierTransform.CreateTransformZeroPadded();
        }

        public AudioContainer AudioContainer { get; }

        public bool IsFinished { get; private set; }

        public float ProgressRatio { get; private set; }

        public ScalogramContainer CreateScalogram(List<float> frequenciesToAnalyze, float cyclesCount = 3)
        {
            ProgressRatio = 0;
            _frequenciesToAnalyze = frequenciesToAnalyze;
            var container = new ScalogramContainer(_signalFT.Length, frequenciesToAnalyze.Count);

            //Parallel.ForEach(frequenciesToAnalyze, frequency =>
            //{
            //Interlocked.Increment(ref _frequenciesCalculated);
            //});

            for (int frequencyIndex = 0; frequencyIndex < frequenciesToAnalyze.Count; frequencyIndex++)
            {
                var frequency = frequenciesToAnalyze[frequencyIndex];
                var sigma = 6;
                var scalogram = WaveletTransformCPU.CreateScalogram(frequency, _signalFT, AudioContainer.SampleRate, cyclesCount, sigma);
                for (int i = 0; i < scalogram.Length / 2; i++)
                {
                    var temp = scalogram[i];
                    scalogram[i] = scalogram[scalogram.Length / 2 + i];
                    scalogram[scalogram.Length / 2 + i] = temp;
                }
                container.SetFrequencyData(frequencyIndex, scalogram);

                ProgressRatio = (float)(frequencyIndex + 1) / frequenciesToAnalyze.Count;
            }

            container.Normalize();
            IsFinished = true;
            return container;
        }
    }
}
