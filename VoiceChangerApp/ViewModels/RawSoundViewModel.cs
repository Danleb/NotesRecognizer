using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using VoiceChanger.FormatParser;
using VoiceChangerApp.Models;
using VoiceChangerApp.Views.SoundViews;

namespace VoiceChangerApp.ViewModels
{
    public class RawSoundViewModel : BindableBase
    {
        public RawSoundViewModel()
        {

        }

        public RawSoundViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
            SoundDataModel.OnSoundTrackLoaded
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnSoundTrackLoaded);

            OnSoundTrackLoaded(SoundDataModel.IsAudioContainerCreated);
        }

        private void OnSoundTrackLoaded(bool success)
        {
            AudioContainer = SoundDataModel.IsAudioContainerCreated ? SoundDataModel.AudioContainer : null;
        }

        private AudioContainer _audioContainer;
        public AudioContainer AudioContainer
        {
            get { return _audioContainer; }
            set { SetProperty(ref _audioContainer, value); }
        }

        private SoundViewPosition _soundViewPosition;
        public SoundViewPosition SoundViewPosition
        {
            get { return _soundViewPosition; }
            set { SetProperty(ref _soundViewPosition, value); }
        }

        public SoundDataModel SoundDataModel { get; }
    }
}
