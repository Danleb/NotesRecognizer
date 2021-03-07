using System.Windows;

namespace VoiceChangerApp.Views
{
    public partial class EditorWindow : Window
    {
        public EditorWindow()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RawSoundView.SetSelected(TabControl.SelectedContent is RawSoundView);
            //SpectrumView.SetSelected(TabControl.SelectedContent is );
        }
    }
}
