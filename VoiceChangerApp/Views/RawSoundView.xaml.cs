using System.Windows.Controls;

namespace VoiceChangerApp.Views
{
    public partial class RawSoundView : UserControl
    {
        public RawSoundView()
        {
            InitializeComponent();
        }

        public void SetSelected(bool isSelected)
        {
            SignalgramView.SetSelected(isSelected);
        }
    }
}
