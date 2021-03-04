using Prism.Mvvm;

namespace VoiceChangerApp.ViewModel
{
    public class EditorViewModel : BindableBase
    {
        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                SetProperty(ref _path, value);
            }
        }


    }
}
