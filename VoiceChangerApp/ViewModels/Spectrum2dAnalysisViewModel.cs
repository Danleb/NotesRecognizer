using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Models;

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

            GenerateSpectrum2dCommand = new DelegateCommand(() =>
            {
                SoundDataModel.GenerateCommonSignalSpectrum();
            });

            SoundDataModel.OnCommonSignalSpectrumGenerated
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnCommonSignalSpectrumGenerated);
        }

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
        public DelegateCommand GenerateSpectrum2dCommand { get; }

        private void OnCommonSignalSpectrumGenerated(bool success)
        {
            if (success)
            {
                CommonSignalSpectrum = SoundDataModel.CommonSignalSpectrum;
            }
        }
    }
}
