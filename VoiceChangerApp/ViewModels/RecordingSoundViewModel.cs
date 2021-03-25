using Prism.Commands;
using Prism.Mvvm;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class RecordingSoundViewModel : BindableBase
    {
        public RecordingSoundViewModel()
        {

        }

        public RecordingSoundViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;

            StartRecording = new DelegateCommand(() =>
            {

            });
            StopRecording = new DelegateCommand(() =>
            {

            });
        }

        private SoundDataModel _soundDataModel;
        public SoundDataModel SoundDataModel
        {
            get { return _soundDataModel; }
            set { SetProperty(ref _soundDataModel, value); }
        }

        private bool _isRecording;
        public bool IsRecording
        {
            get { return _isRecording; }
            set { SetProperty(ref _isRecording, value); }
        }

        private float _recordedTime;
        public float RecordedTime
        {
            get { return _recordedTime; }
            set { SetProperty(ref _recordedTime, value); }
        }

        public DelegateCommand StartRecording { get; }
        public DelegateCommand StopRecording { get; }

    }
}
