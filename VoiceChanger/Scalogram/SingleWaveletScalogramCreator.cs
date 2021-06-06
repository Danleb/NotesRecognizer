using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using VoiceChanger.FormatParser;
using VoiceChanger.NoteRecognizer;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;

namespace VoiceChanger.Scalogram
{
    public class SingleWaveletScalogramCreator : ScalogramCreator
    {
        private readonly Complex[] _signalFT;
        private readonly IFourierTransform _fourierTransform;
        private int _frequenciesCalculated;
        private List<FrequencyData> _frequenciesToAnalyze;

        public static void FixScalogramShift(float[] scalogram)
        {
            for (int i = 0; i < scalogram.Length / 2; i++)
            {
                var temp = scalogram[i];
                scalogram[i] = scalogram[scalogram.Length / 2 + i];
                scalogram[scalogram.Length / 2 + i] = temp;
            }
        }

        public SingleWaveletScalogramCreator(AudioContainer audioContainer, IFourierTransform fourierTransform)
        {
            AudioContainer = audioContainer;
            _fourierTransform = fourierTransform;
            _signalFT = _fourierTransform.CreateTransformZeroPadded();
        }

        public AudioContainer AudioContainer { get; }

        public ScalogramContainer CreateScalogram(List<FrequencyData> frequenciesToAnalyze, WaveletTransformSettings settings, bool normalize = true)
        {
            ProgressRatio = 0;
            _frequenciesCalculated = 0;
            _frequenciesToAnalyze = frequenciesToAnalyze;
            var container = new ScalogramContainer(_signalFT.Length);

            var datas = _frequenciesToAnalyze.AsParallel()
                .Select(frequency =>
                {
                    var scalogram = WaveletTransformCPU.CreateScalogram(frequency, _signalFT, AudioContainer.SampleRate, settings);
                    FixScalogramShift(scalogram);
                    Interlocked.Increment(ref _frequenciesCalculated);
                    ProgressRatio = (float)_frequenciesCalculated / frequenciesToAnalyze.Count;
                    return (frequency, scalogram);
                }).ToList();

            foreach (var (frequency, scalogram) in datas)
            {
                container.SetFrequencyData(frequency, scalogram);
            }

            if (normalize)
            {
                container.NormalizeGlobal();
            }

            IsFinished = true;
            return container;
        }
    }
}
