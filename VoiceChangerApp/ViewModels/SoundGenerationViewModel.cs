using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using VoiceChanger.Utils;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public enum GenerationState
    {
        None = 0,
        Generating,
        Generated
    }

    public class SoundGenerationViewModel : BindableBase
    {
        public SoundGenerationViewModel() { }

        public SoundGenerationViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;

            GenerateSample = new DelegateCommand(() =>
            {
                var settings = new SampleGeneratorSettings
                {
                    SampleRate = SampleRate,
                    Datas = new List<SignalGenerationData>
                    {
                        new SignalGenerationData
                        {
                            Duration = Duration,
                            Frequency = SignalFrequency
                        }
                    }
                };
                SoundDataModel.GenerateSample.OnNext(settings);
            });

            LoadToAnalysis = new DelegateCommand(() =>
            {

            });

            SaveToFile = new DelegateCommand(() =>
            {

            });

            SignalFrequency = 1;
            SampleRate = 1024;
            Duration = 128;
        }

        #region Properties

        private int _signalFrequency;
        public int SignalFrequency
        {
            get { return _signalFrequency; }
            set { SetProperty(ref _signalFrequency, value); }
        }

        private float _duration;
        public float Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        private int _sampleRate;
        public int SampleRate
        {
            get { return _sampleRate; }
            set { SetProperty(ref _sampleRate, value); }
        }

        private SoundDataModel _soundDataModel;
        public SoundDataModel SoundDataModel
        {
            get { return _soundDataModel; }
            set { SetProperty(ref _soundDataModel, value); }
        }

        private GenerationState _generationState;
        public GenerationState GenerationState
        {
            get { return _generationState; }
            set { SetProperty(ref _generationState, value); }
        }

        #endregion

        #region Commands

        public DelegateCommand GenerateSample { get; }
        public DelegateCommand LoadToAnalysis { get; }
        public DelegateCommand SaveToFile { get; }

        #endregion
    }
}
