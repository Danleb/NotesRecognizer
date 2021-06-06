using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Immutable;
using System.Linq;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class WaveletGenerationViewModel : BindableBase
    {
        public WaveletGenerationViewModel() { }

        public WaveletGenerationViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;

            GenerateWavelet = new DelegateCommand(() =>
            {
                if (WaveletType == WaveletType.Morlet)
                {
                    //todo: put to wavelet model;
                    var complex = WaveletTransformCPU.GenerateMorletWavelet(WaveFrequency, SampleRate, CyclesCount, PointsCount, SigmaValue);
                    var real = complex.Select(v => (float)v.Real).ToArray();
                    var duration = 1.0 / SampleRate * PointsCount;
                    var container = new AudioContainer((float)duration, SampleRate, real);
                    SoundDataModel.LoadContainer.OnNext(container);
                }
                else if (WaveletType == WaveletType.Guitar)
                {
                    var settings = new GuitarWaveletSettings
                    {
                        Coefficient = 10,
                        Bias = 0.1f,
                        Duration = CyclesCount// 0.5f
                    };
                    var complex = GuitarWaveletCreator.CreateWavelet(WaveFrequency, SampleRate, PointsCount, settings);
                    var real = complex.Select(v => (float)v.Real).ToArray();
                    var duration = 1.0 / SampleRate * PointsCount;
                    var container = new AudioContainer((float)duration, SampleRate, real);
                    SoundDataModel.LoadContainer.OnNext(container);
                }
            });

            WaveFrequency = 10;
            CyclesCount = 5;
            PointsCount = 44100 * 10;
            SampleRate = 44100;
            SigmaValue = 5;

            WaveletType = WaveletType.Guitar;
        }

        #region Properties

        private WaveletType _waveletType;
        public WaveletType WaveletType
        {
            get { return _waveletType; }
            set { SetProperty(ref _waveletType, value); }
        }

        public ImmutableArray<WaveletType> WaveletTypes { get; } = ImmutableArray.Create(WaveletType.Morlet, WaveletType.Guitar);

        private float _waveFrequency;
        public float WaveFrequency
        {
            get { return _waveFrequency; }
            set { SetProperty(ref _waveFrequency, value); }
        }

        private float _cyclesCount;
        public float CyclesCount
        {
            get { return _cyclesCount; }
            set { SetProperty(ref _cyclesCount, value); }
        }

        private int _pointsCount;
        public int PointsCount
        {
            get { return _pointsCount; }
            set { SetProperty(ref _pointsCount, value); }
        }

        private int _sampleRate;
        public int SampleRate
        {
            get { return _sampleRate; }
            set { SetProperty(ref _sampleRate, value); }
        }

        private int _sigmaValue;
        public int SigmaValue
        {
            get { return _sigmaValue; }
            set { SetProperty(ref _sigmaValue, value); }
        }

        #endregion

        public SoundDataModel SoundDataModel { get; }

        #region Commands

        public DelegateCommand GenerateWavelet { get; }

        #endregion
    }
}
