using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VoiceChanger.FormatParser;
using VoiceChanger.NoteRecognizer;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;

namespace VoiceChanger.Scalogram
{
    public class GuitarHitsScalogramCreator : ScalogramCreator
    {
        private readonly AudioContainer _audioContainer;
        private int _frequenciesCalculated;
        private List<FrequencyData> _frequenciesToAnalyze;

        public GuitarHitsScalogramCreator(AudioContainer audioContainer)
        {
            _audioContainer = audioContainer;
        }

        public ScalogramContainer CreateScalogram(List<FrequencyData> frequenciesToAnalyze, GuitarHitsScalogramSettings settings, bool normalize)
        {
            ProgressRatio = 0;
            _frequenciesCalculated = 0;
            _frequenciesToAnalyze = frequenciesToAnalyze;

            var signalFt = new FastFourierTransformCPU(_audioContainer.Samples).CreateTransformZeroPadded();

            var container = new ScalogramContainer(signalFt.Length);

            var datas = _frequenciesToAnalyze.AsParallel()
                .Select(frequency =>
                {
                    var guitarWaveletSettings = new GuitarWaveletSettings
                    {
                        Duration = settings.Duration
                    };
                    var bandPassScalogram = WaveletTransformCPU.CreateScalogram(frequency, signalFt, _audioContainer.SampleRate, guitarWaveletSettings);
                    SingleWaveletScalogramCreator.FixScalogramShift(bandPassScalogram);

                    var bandPassedSignalFt = new FastFourierTransformCPU(bandPassScalogram).CreateTransformZeroPadded();

                    var morletWaveletSettings = new MorletWaveletSettings
                    {
                        CyclesCount = settings.CyclesCount
                    };
                    var scalogram = WaveletTransformCPU.CreateScalogram(frequency, bandPassedSignalFt, _audioContainer.SampleRate, morletWaveletSettings);
                    SingleWaveletScalogramCreator.FixScalogramShift(scalogram);

                    container.Plots[frequency] = bandPassScalogram;

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
                //container.NormalizeEachLocally();
                container.NormalizeGlobal();
            }

            IsFinished = true;
            return container;
        }
    }
}
