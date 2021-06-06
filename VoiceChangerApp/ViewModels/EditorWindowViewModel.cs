using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;
using VoiceChangerApp.Views;

namespace VoiceChangerApp.ViewModels
{
    public class EditorWindowViewModel : BindableBase
    {
        private readonly ErrorModel _errorModel;
        private readonly SoundDataModel _soundDataModel;

        public EditorWindowViewModel() { }

        public EditorWindowViewModel(ErrorModel errorModel, SoundDataModel soundDataModel)
        {
            _errorModel = errorModel;
            _soundDataModel = soundDataModel;

            ToolbarOpenAppSettings = new DelegateCommand(() =>
            {
                new AppSettingsView().ShowDialog();
            });

            _errorModel.OnErrorDescription
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(e =>
                {
                    MessageBox.Show(e, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            _soundDataModel.OnSampleLoaded
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(loaded =>
                {
                    switch (_soundDataModel.SoundSource)
                    {
                        case SoundSource.File:
                            {
                                Title = _soundDataModel.FilePath;
                                break;
                            }
                        default:
                            {
                                Title = _soundDataModel.SoundSource.ToString();
                                break;
                            }
                    }
                });

            Title = "Suprecessor";
        }

        #region Commands

        public DelegateCommand ToolbarOpenAppSettings { get; }

        #endregion

        #region Properties

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        #endregion
    }
}
