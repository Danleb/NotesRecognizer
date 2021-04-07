using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using VoiceChanger.Utils;
using VoiceChangerApp.Models;
using VoiceChanger.SpectrumCreator;
using System.Linq;
using VoiceChanger.FormatParser;

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
                //todo: put to wavelet model;
                var complex = WaveletTransformCPU.GenerateMorletWavelet(WaveFrequency, SampleRate, CyclesCount, PointsCount, SigmaValue);
                var real = complex.Select(v => (float)v.Real).ToArray();
                var duration = 1.0 / SampleRate * PointsCount;
                var container = new AudioContainer((float)duration, SampleRate, real);
                SoundDataModel.LoadContainer.OnNext(container);                
            });

            WaveFrequency = 1;
            CyclesCount = 5;
            PointsCount = 1024;
            SampleRate = 64;
            SigmaValue = 5;
        }

        #region Properties

        private int _waveFrequency;
        public int WaveFrequency
        {
            get { return _waveFrequency; }
            set { SetProperty(ref _waveFrequency, value); }
        }

        private int _cyclesCount;
        public int CyclesCount
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
