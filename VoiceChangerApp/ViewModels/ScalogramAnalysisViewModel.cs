using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
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
                var list = new List<float>();
                for (var frequency = FrequencyFrom; frequency <= FrequencyTo; frequency += FrequencyStep)
                {
                    list.Add(frequency);
                }
                ScalogramModel.CreateScalogram.OnNext(list);
            });

            ScalogramType = ScalogramType.Linear;
            FrequencyFrom = 100;
            FrequencyTo = 200;
            FrequencyStep = 1;
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
            set { SetProperty(ref _scalogramType, value); }
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

        #endregion

        #region Commands

        public DelegateCommand CreateScalogram { get; }

        #endregion
    }
}
