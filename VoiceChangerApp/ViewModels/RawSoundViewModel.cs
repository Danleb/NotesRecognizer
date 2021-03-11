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

            var sc2 = SynchronizationContext.Current;
            var sc = FileInfoViewModel.sc;
            var eq = sc == sc2;

            SoundDataModel.OnSoundTrackLoaded
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ =>
                {
                    AudioContainer = SoundDataModel.IsAudioContainerCreated ? SoundDataModel.AudioContainer : null;
                });
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
