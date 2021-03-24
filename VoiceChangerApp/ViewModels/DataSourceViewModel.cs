using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class DataSourceViewModel : BindableBase
    {
        public DataSourceViewModel()
        {

        }

        public DataSourceViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
            OpenFileCommand = new DelegateCommand(() =>
            {
                var openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    SoundDataModel.OnLoadFile.OnNext(openFileDialog.FileName);
                }
            });

            SoundDataModel.OnSoundTrackLoaded
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnSoundTrackLoaded);

            OnSoundTrackLoaded(SoundDataModel.IsAudioContainerCreated);
        }

        private void OnSoundTrackLoaded(bool success)
        {
            IsSoundTrackLoaded = SoundDataModel.IsAudioContainerCreated;
            IsLoadedFromFile = SoundDataModel.IsLoadedFromFile;
            if (IsLoadedFromFile)
            {
                Path = SoundDataModel.Path;
                Duration = SoundDataModel.AudioContainer.Duration;
                SampleRate = SoundDataModel.AudioContainer.SampleRate;
                SignalsCount = SoundDataModel.AudioContainer.Samples.Length;
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        private bool _isSoundTrackLoaded;
        public bool IsSoundTrackLoaded
        {
            get { return _isSoundTrackLoaded; }
            set { SetProperty(ref _isSoundTrackLoaded, value); }
        }

        private bool _isLoadedFromFile;
        public bool IsLoadedFromFile
        {
            get { return _isLoadedFromFile; }
            set { SetProperty(ref _isLoadedFromFile, value); }
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

        private int _signalsCount;
        public int SignalsCount
        {
            get { return _signalsCount; }
            set { SetProperty(ref _signalsCount, value); }
        }

        public DelegateCommand OpenFileCommand { get; private set; }

        public SoundDataModel SoundDataModel { get; }
    }
}
