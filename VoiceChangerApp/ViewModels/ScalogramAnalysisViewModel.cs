using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using VoiceChanger.Scalogram;
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.ViewModels
{
    public class ScalogramAnalysisViewModel : BindableBase
    {
        private readonly DispatcherTimer _dispatcherTimer = new();
        private DateTime _startTime;

        public ScalogramAnalysisViewModel() { }

        public ScalogramAnalysisViewModel(ScalogramModel scalogramModel)
        {
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 25);

            ScalogramModel = scalogramModel;

            CreateScalogram = new DelegateCommand(() =>
            {
                switch (_scalogramType)
                {
                    case ScalogramType.Guitar:
                        {
                            var settings = new GuitarScalogramCreationSettings
                            {
                                TonesCount = GuitarTonesCount,
                                CyclesCount = CyclesCount
                            };
                            ScalogramModel.CreateScalogramGuitar.OnNext(settings);
                            break;
                        }
                    case ScalogramType.Linear:
                        {
                            var settings = new LinearScalogramCreationSettings
                            {
                                FrequencyFrom = FrequencyFrom,
                                FrequencyTo = FrequencyTo,
                                FrequencyStep = FrequencyStep,
                                CyclesCount = CyclesCount
                            };
                            ScalogramModel.CreateScalogramLinear.OnNext(settings);
                            break;
                        }
                    case ScalogramType.Harmonics:
                        {
                            var settings = new HarmonicsScalogramCreationSettings
                            {
                                BaseFrequency = BaseFrequency,
                                HarmomicsCount = HarmonicsCount,
                                CyclesCount = CyclesCount
                            };
                            ScalogramModel.CreateScalogram.OnNext(settings);
                            break;
                        }
                }
            });

            ScalogramModel.OnScalogramCreated
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnScalogramCreated);
            ScalogramModel.OnScalogramCalculationState
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnScalogramCalculationStateChanged);

            CyclesCount = 2;

            ScalogramType = ScalogramType.Linear;
            FrequencyFrom = 100;
            FrequencyTo = 200;
            FrequencyStep = 1;

            GuitarTonesCount = 2;
        }

        public ScalogramModel ScalogramModel { get; }

        #region Properties

        private ScalogramContainer _scalogramContainer;
        public ScalogramContainer ScalogramContainer
        {
            get { return _scalogramContainer; }
            set { SetProperty(ref _scalogramContainer, value); }
        }

        public List<ScalogramType> AvailableScalogramTypes { get; } = new List<ScalogramType> {
            ScalogramType.Guitar,
            ScalogramType.Linear
        };

        private ScalogramType _scalogramType;
        public ScalogramType ScalogramType
        {
            get { return _scalogramType; }
            set
            {
                SetProperty(ref _scalogramType, value);
                GuitarSettingsVisible = _scalogramType == ScalogramType.Guitar;
                LinearSettingsVisible = _scalogramType == ScalogramType.Linear;
            }
        }

        private float _frequencyFrom;
        public float FrequencyFrom
        {
            get { return _frequencyFrom; }
            set { SetProperty(ref _frequencyFrom, value); }
        }

        private float _frequencyTo;
        public float FrequencyTo
        {
            get { return _frequencyTo; }
            set { SetProperty(ref _frequencyTo, value); }
        }

        private float _frequencyStep;
        public float FrequencyStep
        {
            get { return _frequencyStep; }
            set { SetProperty(ref _frequencyStep, value); }
        }

        private int _guitarTonesCount;
        public int GuitarTonesCount
        {
            get { return _guitarTonesCount; }
            set { SetProperty(ref _guitarTonesCount, value); }
        }

        private bool _guitarSettingsVisible;
        public bool GuitarSettingsVisible
        {
            get { return _guitarSettingsVisible; }
            set { SetProperty(ref _guitarSettingsVisible, value); }
        }

        private bool _linearSettingsVisible;
        public bool LinearSettingsVisible
        {
            get { return _linearSettingsVisible; }
            set { SetProperty(ref _linearSettingsVisible, value); }
        }

        private double _secondsPassed;
        public double SecondsPassed

        {
            get { return _secondsPassed; }
            set { SetProperty(ref _secondsPassed, value); }
        }

        private float _progressRatio;
        public float ProgressPercents
        {
            get { return _progressRatio; }
            set { SetProperty(ref _progressRatio, value); }
        }

        private CalculationState _calculationState;
        public CalculationState CalculationState
        {
            get { return _calculationState; }
            set { SetProperty(ref _calculationState, value); }
        }

        private float _cyclesCount;
        public float CyclesCount
        {
            get { return _cyclesCount; }
            set { SetProperty(ref _cyclesCount, value); }
        }

        #endregion

        #region Commands

        public DelegateCommand CreateScalogram { get; }

        #endregion

        private void OnScalogramCreated(ScalogramContainer scalogramContainer)
        {
            ScalogramContainer = scalogramContainer;
            _dispatcherTimer.Stop();
        }

        private void OnScalogramCalculationStateChanged(CalculationState state)
        {
            CalculationState = state;

            switch (state)
            {
                case CalculationState.Calculating:
                    {
                        ProgressPercents = 0;
                        SecondsPassed = 0;
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
            ProgressPercents = ScalogramModel.ScalogramCreator?.ProgressRatio * 100 ?? 0;
        }
    }
}
