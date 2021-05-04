using System.Reactive.Subjects;
using VoiceChanger.FormatParser;
using VoiceChangerApp.Utils;
using System;

namespace VoiceChangerApp.Models
{
    public class AudioPlayerModel
    {
        private IErrorModel _errorModel;

        public AudioPlayerModel(IErrorModel errorModel)
        {
            _errorModel = errorModel;

            Play.SubscribeAsync(PlayImpl);
        }

        #region Properties

        public AudioContainer AudioContainer { get; set; }
        public float CurrentTime { get; set; }

        #endregion

        #region Commands

        public Subject<bool> Play { get; } = new();
        public Subject<AudioContainer> SetAudioContainer { get; } = new();

        #endregion

        #region Events

        public Subject<bool> OnPlayingStarted { get; } = new();
        public Subject<AudioContainer> OnAudioContainerChanged { get; } = new();

        #endregion

        private void PlayImpl(bool _)
        {
            if (AudioContainer == null)
            {
                return;
            }


        }

        private void SetAudioContainerImpl(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer ?? throw new ArgumentNullException(nameof(audioContainer));
        }
    }
}
