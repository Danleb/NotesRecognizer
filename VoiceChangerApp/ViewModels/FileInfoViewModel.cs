using Microsoft.Win32;
using Prism.Commands;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class FileInfoViewModel
    {
        public FileInfoViewModel()
        {

        }

        public FileInfoViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
            OpenFileCommand = new DelegateCommand(() =>
            {
                var openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    SoundDataModel.LoadFile(openFileDialog.FileName);
                }
            });
        }

        public string Path { get; private set; } = "BIND";

        public DelegateCommand OpenFileCommand { get; private set; }

        public SoundDataModel SoundDataModel { get; }
    }
}
