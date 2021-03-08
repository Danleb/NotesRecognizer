using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Windows;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class EditorWindowViewModel : BindableBase
    {
        public EditorWindowViewModel()
        {

        }

        public EditorWindowViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;

            SoundDataModel.OnException.Subscribe(e =>
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public SoundDataModel SoundDataModel { get; }
    }
}
