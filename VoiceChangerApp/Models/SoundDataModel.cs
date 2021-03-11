using Microsoft.Extensions.Logging;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;

namespace VoiceChangerApp.Models
{
    public class SoundDataModel : BindableBase
    {
        private readonly ILogger _logger;

        public SoundDataModel(ILogger<SoundDataModel> logger)
        {
            _logger = logger;

            OnLoadFile.Subscribe(LoadFile);
            LoadDefaultFile();
        }

        #region Events

        public Subject<string> OnLoadFile = new();
        public Subject<bool> OnSoundTrackLoaded = new();
        public Subject<Exception> OnException = new();
        public Subject<bool> OnIsLoading = new();

        #endregion

        #region Data

        public AudioContainer AudioContainer { get; private set; }
        public SpectrumCreator SpectrumCreator { get; private set; }
        public SpectrumContainer SpectrumContainer => SpectrumCreator.SpectrumContainer;
        public bool IsAudioContainerCreated => AudioContainer != null;
        public bool IsLoadedFromFile { get; private set; }
        public string Path { get; private set; }

        #endregion

        private void LoadFile(string path)
        {
            Path = path;
            IsLoadedFromFile = true;
            OnIsLoading.OnNext(true);

            try
            {
                _logger.LogInformation("Loading from path {Path}", path);
                AudioContainer = AudioLoader.Load(path);
                OnSoundTrackLoaded.OnNext(true);
            }
            catch (Exception e)
            {
                OnSoundTrackLoaded.OnNext(false);
                OnException.OnNext(e);
            }
            finally
            {
                OnIsLoading.OnNext(false);
            }
        }

        [Conditional("DEBUG")]
        private void LoadDefaultFile()
        {
            OnLoadFile.OnNext(Samples.Wave100hz);
        }
    }
}
