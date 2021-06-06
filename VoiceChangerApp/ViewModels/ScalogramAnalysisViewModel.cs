using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using VoiceChanger.Scalogram;
using VoiceChanger.Utils;
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
                ScalogramCreationSettings settings = null;

                switch (_scalogramType)
                {
                    case ScalogramType.Guitar:
                        {
                            settings = new GuitarScalogramCreationSettings
                            {
                                TonesCount = GuitarTonesCount
                            };
                            break;
                        }
                    case ScalogramType.Linear:
                        {
                            settings = new LinearScalogramCreationSettings
                            {
                                FrequencyFrom = FrequencyFrom,
                                FrequencyTo = FrequencyTo,
                                FrequencyStep = FrequencyStep
                            };
                            break;
                        }
                    case ScalogramType.Harmonics:
                        {
                            settings = new HarmonicsScalogramCreationSettings
                            {
                                ByStringNumber = true,
                                StringNumber = HarmonicsStringNumber,
                                ToneIndex = HarmonicsToneIndex,
                                HarmomicsCount = HarmonicsCount
                            };
                            break;
                        }
                    case ScalogramType.GuitarHits:
                        {
                            settings = new GuitarHitsScalogramCreationSettings
                            {
                                GuitarHitsScalogramSettings = new GuitarHitsScalogramSettings
                                {
                                    CyclesCount = CyclesCount,
                                    Duration = Duration
                                }
                            };
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }

                settings.WaveletTransformSettings = new GuitarWaveletSettings
                {
                    Coefficient = Coefficient,
                    Bias = Bias,
                    Duration = Duration
                };
                //settings.WaveletTransformSettings = new MorletWaveletSettings
                //{
                //    CyclesCount = CyclesCount
                //};

                ScalogramModel.CreateScalogram.OnNext(settings);
            });

            ScalogramModel.OnScalogramCreated
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnScalogramCreated);
            ScalogramModel.OnScalogramCalculationState
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnScalogramCalculationStateChanged);

            CyclesCount = 0.2f;

            ScalogramType = ScalogramType.Linear;
            FrequencyFrom = 82.41f;
            FrequencyTo = 82.41f;
            FrequencyStep = 1;

            GuitarTonesCount = 2;

            HarmonicsStringNumber = 6;
            HarmonicsToneIndex = 0;
            HarmonicsCount = 4;

            Coefficient = 2f;
            Bias = 0.1f;
            Duration = 0.05f;
        }

        public ScalogramModel ScalogramModel { get; }

        #region Properties

        private ScalogramContainer _scalogramContainer;
        public ScalogramContainer ScalogramContainer
        {
            get { return _scalogramContainer; }
            set { SetProperty(ref _scalogramContainer, value); }
        }

        public ImmutableArray<ScalogramType> AvailableScalogramTypes { get; } = ImmutableArray.Create(
            ScalogramType.Guitar,
            ScalogramType.Linear,
            ScalogramType.Harmonics,
            ScalogramType.GuitarHits
        );

        public ImmutableArray<int> StringNumbers { get; } = ImmutableArray.Create(1, 2, 3, 4, 5, 6);

        public ImmutableArray<int> StringTones { get; } = ImmutableArray.Create(Enumerable.Range(0, 13).ToArray());

        private ScalogramType _scalogramType;
        public ScalogramType ScalogramType
        {
            get { return _scalogramType; }
            set
            {
                SetProperty(ref _scalogramType, value);
                GuitarSettingsVisible = _scalogramType == ScalogramType.Guitar;
                LinearSettingsVisible = _scalogramType == ScalogramType.Linear || _scalogramType == ScalogramType.GuitarHits;
                HarmonicsSettingsVisible = _scalogramType == ScalogramType.Harmonics;
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

        private bool _harmonicsSettingsVisible;
        public bool HarmonicsSettingsVisible
        {
            get { return _harmonicsSettingsVisible; }
            set { SetProperty(ref _harmonicsSettingsVisible, value); }
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

        private int _harmonicsStringNumber;
        public int HarmonicsStringNumber
        {
            get { return _harmonicsStringNumber; }
            set { SetProperty(ref _harmonicsStringNumber, value); }
        }

        private int _harmonicsToneIndex;
        public int HarmonicsToneIndex
        {
            get { return _harmonicsToneIndex; }
            set { SetProperty(ref _harmonicsToneIndex, value); }
        }

        private int _harmonicsCount;
        public int HarmonicsCount
        {
            get { return _harmonicsCount; }
            set { SetProperty(ref _harmonicsCount, value); }
        }

        private float _coeff;
        public float Coefficient
        {
            get { return _coeff; }
            set { SetProperty(ref _coeff, value); }
        }

        private float _bias;
        public float Bias
        {
            get { return _bias; }
            set { SetProperty(ref _bias, value); }
        }

        private float _duration;
        public float Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
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
                        UpdateProgressPercents();
                        break;
                    }
                default: throw new NotImplementedException();
            }
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var delta = DateTime.Now - _startTime;
            SecondsPassed = delta.TotalSeconds;
            UpdateProgressPercents();
        }

        private void UpdateProgressPercents()
        {
            ProgressPercents = ScalogramModel.ScalogramCreator?.ProgressRatio * 100 ?? 0;
        }
    }
}
