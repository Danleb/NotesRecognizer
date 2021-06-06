using VoiceChangerApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using VoiceChanger.FormatParser;
using VoiceChangerApp.Utils;
using System.Reactive.Subjects;

namespace VoiceChangerApp.ViewModels
{
    public class AudioPlayerViewModel
    {
        private readonly AudioPlayerModel _audioPlayerModel;

        public AudioPlayerViewModel(AudioPlayerModel audioPlayerModel)
        {
            _audioPlayerModel = audioPlayerModel;


        }

        #region Events

        public Subject<float> OnCurrentTimeChanged { get; } = new();
        public Subject<bool> OnPlayingStateChanged { get; } = new();
        public Subject<AudioContainer> OnAudioContainerChanged { get; } = new();

        #endregion
    }
}
