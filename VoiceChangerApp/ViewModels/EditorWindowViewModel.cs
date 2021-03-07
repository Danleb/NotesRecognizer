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

            Observable.FromEvent<Exception>(
                v =>
                {
                    SoundDataModel.OnException += v;
                },
                v =>
                {
                    SoundDataModel.OnException -= v;
                }).Subscribe(
                e =>
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });

            //SoundDataModel.OnException.AsObservable().Subscribe(
            //e =>
            //{
            //    MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //});

            SoundDataModel.SoundTrackLoaded.AsObservable().Subscribe(v =>
            {
                Console.WriteLine("!!!!!");
            });
        }

        public SoundDataModel SoundDataModel { get; }
    }
}
