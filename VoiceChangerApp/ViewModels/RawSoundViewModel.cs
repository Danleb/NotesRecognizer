using Prism.Mvvm;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class RawSoundViewModel : BindableBase
    {
        public RawSoundViewModel()
        {

        }

        public RawSoundViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
        }

        public SoundDataModel SoundDataModel { get; }
    }
}
