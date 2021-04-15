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
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace VoiceChangerApp.ViewModels
{
    public class DataSourceViewModel : BindableBase
    {
        public DataSourceViewModel() { }

        public DataSourceViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
            OpenFileCommand = new DelegateCommand(() =>
            {
                var openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    SoundDataModel.LoadFile.OnNext(openFileDialog.FileName);
                }
            });
            SelectWorkFolder = new DelegateCommand(() =>
            {
                using var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SoundDataModel.SetCurrentWorkDirectory.OnNext(dialog.SelectedPath);
                }
            });
            LoadSelectedFile = new DelegateCommand(() =>
            {
                SoundDataModel.LoadFile.OnNext(SelectedFilePathItem);
            });

            SoundDataModel.OnSampleLoaded
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnSoundTrackLoaded);

            SoundDataModel.OnSoundSourceChanged
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnSoundSourceChanged);

            SoundDataModel.OnCurrentWorkDirectoryChanged
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnCurrentWorkDirectoryChanged);

            OnSoundTrackLoaded(SoundDataModel.IsAudioContainerCreated);
            OnSoundSourceChanged(soundDataModel.SoundSource);
        }

        #region Properties

        public DelegateCommand OpenFileCommand { get; private set; }

        public SoundDataModel SoundDataModel { get; }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
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

        private SoundSource _soundSource;
        public SoundSource SoundSource
        {
            get { return _soundSource; }
            set { SetProperty(ref _soundSource, value); }
        }

        private string _currentWorkDirectory;
        public string CurrentWorkDirectory
        {
            get { return _currentWorkDirectory; }
            set { SetProperty(ref _currentWorkDirectory, value); }
        }

        private List<string> _directoryFiles;
        public List<string> DirectoryFiles
        {
            get { return _directoryFiles; }
            set { SetProperty(ref _directoryFiles, value); }
        }

        private string _selectedFilePathItem;
        public string SelectedFilePathItem
        {
            get { return _selectedFilePathItem; }
            set { SetProperty(ref _selectedFilePathItem, value); }
        }

        private int _loadedChannelNumber;
        public int ChannelNumber
        {
            get { return _loadedChannelNumber; }
            set { SetProperty(ref _loadedChannelNumber, value); }
        }

        #endregion

        #region Methods

        private void OnCurrentWorkDirectoryChanged(string path)
        {
            CurrentWorkDirectory = path;
            DirectoryFiles = Directory.EnumerateFiles(CurrentWorkDirectory)
                .Where(v => AudioLoader.SupportedExtensions.Contains(Path.GetExtension(v)))
                .ToList();
        }

        private void OnSoundSourceChanged(SoundSource soundSource)
        {
            SoundSource = soundSource;
        }

        private void OnSoundTrackLoaded(bool success)
        {
            IsSoundTrackLoaded = SoundDataModel.IsAudioContainerCreated;
            IsLoadedFromFile = SoundDataModel.SoundSource == SoundSource.File;
            if (SoundDataModel.IsAudioContainerCreated)
            {
                FilePath = SoundDataModel.FilePath;
                Duration = SoundDataModel.AudioContainer.Duration;
                SampleRate = SoundDataModel.AudioContainer.SampleRate;
                SignalsCount = SoundDataModel.AudioContainer.Samples.Length;
            }
        }

        #endregion

        #region Commands

        public DelegateCommand SelectWorkFolder { get; }
        public DelegateCommand LoadSelectedFile { get; }

        #endregion
    }
}
