using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.ViewModels
{
    public class Spectrum2dAnalysisViewModel : BindableBase
    {
        public Spectrum2dAnalysisViewModel()
        {

        }

        public Spectrum2dAnalysisViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;

            CalculateSpectrum2dCommand = new DelegateCommand(() =>
            {
                SoundDataModel.CalculateSampleSignalSpectrum.Invoke();
            });

            SoundDataModel.OnCommonSignalSpectrumCalculated
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnCommonSignalSpectrumCalculated);
        }

        #region Properties

        private SpectrumSlice _spectrumSlice;
        public SpectrumSlice CommonSignalSpectrum
        {
            get { return _spectrumSlice; }
            set { SetProperty(ref _spectrumSlice, value); }
        }
        private SoundDataModel _soundDataModel;
        public SoundDataModel SoundDataModel
        {
            get { return _soundDataModel; }
            set { SetProperty(ref _soundDataModel, value); }
        }
        private bool _isSpectrumGenerated;
        public bool IsSpectrumGenerated
        {
            get { return _isSpectrumGenerated; }
            set { SetProperty(ref _isSpectrumGenerated, value); }
        }

        private string _frequenciesRange;
        public string FrequenciesRange
        {
            get { return _frequenciesRange; }
            set { SetProperty(ref _frequenciesRange, value); }
        }

        #endregion

        #region Commands

        public DelegateCommand CalculateSpectrum2dCommand { get; }

        #endregion

        private void OnCommonSignalSpectrumCalculated(bool success)
        {
            IsSpectrumGenerated = success;
            if (success)
            {
                CommonSignalSpectrum = SoundDataModel.CommonSignalSpectrum;
                FrequenciesRange = $"{CommonSignalSpectrum.MinAmplitude}-{CommonSignalSpectrum.MaxFrequency}";
            }
        }
    }
}
