using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class FileInfoViewModel : BindableBase
    {
        public FileInfoViewModel()
        {

        }

        public static SynchronizationContext sc = null;

        public FileInfoViewModel(SoundDataModel soundDataModel)
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

            sc = SynchronizationContext.Current;

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

        public DelegateCommand OpenFileCommand { get; private set; }

        public SoundDataModel SoundDataModel { get; }
    }
}
