using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using VoiceChanger.Scalogram;
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.ViewModels
{
    public class ScalogramAnalysisViewModel : BindableBase
    {
        public ScalogramAnalysisViewModel() { }

        public ScalogramAnalysisViewModel(ScalogramModel scalogramModel)
        {
            ScalogramModel = scalogramModel;

            CreateScalogram = new DelegateCommand(() =>
            {
                switch (_scalogramType)
                {
                    case ScalogramType.Guitar:
                        {
                            var settings = new GuitarScalogramCreationSettings
                            {
                                TonesCount = GuitarTonesCount
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
                                FrequencyStep = FrequencyStep
                            };
                            ScalogramModel.CreateScalogramLinear.OnNext(settings);
                            break;
                        }
                }
            });

            ScalogramModel.OnScalogramCreated
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnScalogramCreated);

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

        #endregion

        #region Commands

        public DelegateCommand CreateScalogram { get; }

        #endregion

        private void OnScalogramCreated(ScalogramContainer scalogramContainer)
        {
            ScalogramContainer = scalogramContainer;
        }
    }
}
