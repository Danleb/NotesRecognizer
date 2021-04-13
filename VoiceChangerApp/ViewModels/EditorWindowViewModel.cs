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

        public EditorWindowViewModel(ErrorModel errorModel)
        {
            ErrorModel = errorModel;

            ErrorModel.OnErrorDescription.Subscribe(e =>
            {
                MessageBox.Show(e, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public ErrorModel ErrorModel { get; }
    }
}
