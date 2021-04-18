using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.ViewModels
{
    public class Spectrum2dAnalysisViewModel : BindableBase
    {
        private readonly DispatcherTimer _dispatcherTimer = new();
        private DateTime _startTime;

        public Spectrum2dAnalysisViewModel() { }

        public Spectrum2dAnalysisViewModel(SoundDataModel soundDataModel)
        {
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 25);

            SoundDataModel = soundDataModel;

            CalculateSpectrum2dCommand = new DelegateCommand(() =>
            {
                SoundDataModel.CalculateSampleSignalSpectrum.Invoke();
            });

            SoundDataModel.OnCommonSignalSpectrumCalculated
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnCommonSignalSpectrumCalculated);
            SoundDataModel.OnCommonSignalSpectrumCalculationState
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnCalculationStateChanged);
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

        private CalculationState _calculationState;
        public CalculationState CalculationState
        {
            get { return _calculationState; }
            set { SetProperty(ref _calculationState, value); }
        }

        private double _secondsPassed;
        public double SecondsPassed
        {
            get { return _secondsPassed; }
            set { SetProperty(ref _secondsPassed, value); }
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

        private void OnCalculationStateChanged(CalculationState calculationState)
        {
            CalculationState = calculationState;

            switch (calculationState)
            {
                case CalculationState.Calculating:
                    {
                        _dispatcherTimer.Start();
                        _startTime = DateTime.Now;
                        break;
                    }
                case CalculationState.Finished:
                case CalculationState.Cancelled:
                case CalculationState.ErrorHappened:
                case CalculationState.None:
                    {
                        _dispatcherTimer.Stop();
                        break;
                    }
                default: throw new NotImplementedException();
            }
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var delta = DateTime.Now - _startTime;
            SecondsPassed = delta.TotalSeconds;
        }
    }
}
