using Prism.Commands;
using Prism.Mvvm;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class SoundGenerationViewModel : BindableBase
    {
        public SoundGenerationViewModel()
        {
            GenerateSample = new DelegateCommand(() =>
            {


            });
        }

        public SoundGenerationViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
        }

        private SoundDataModel _soundDataModel;
        public SoundDataModel SoundDataModel
        {
            get { return _soundDataModel; }
            set { SetProperty(ref _soundDataModel, value); }
        }
        public DelegateCommand GenerateSample { get; }
    }
}
