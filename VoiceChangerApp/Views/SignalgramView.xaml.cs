using SharpGL;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using VoiceChanger.FormatParser;

namespace VoiceChangerApp.Views
{
    /// <summary>
    /// Interaction logic for SignalgramView.xaml
    /// </summary>
    public partial class SignalgramView : UserControl
    {
        public static readonly DependencyProperty AudioContainerProperty = DependencyProperty.Register(
            nameof(AudioContainer),
            typeof(AudioContainer),
            typeof(SignalgramView),
            new UIPropertyMetadata(null));

        public SignalgramView()
        {
            InitializeComponent();
            OpenGLControl.RenderContextType = RenderContextType.HiddenWindow;
        }

        public void SetSelected(bool isSelected)
        {
            OpenGLControl.RenderContextType = isSelected ? RenderContextType.DIBSection : RenderContextType.HiddenWindow;
        }

        [Bindable(true)]
        public AudioContainer AudioContainer
        {
            get { return (AudioContainer)GetValue(AudioContainerProperty); }
            set { SetValue(AudioContainerProperty, value); }
        }

        public void ResetView()
        {

        }

        private void OpenGLControl_Initialized(object sender, System.EventArgs e)
        {

        }

        private void OpenGLControl_Resized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {

        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
            if (AudioContainer == null)
            {
                gl.ClearColor(0.1f, 1f, 0f, 1f);
            }
            else
            {
                gl.ClearColor(0f, 0f, 1f, 1f);
            }

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);


        }
    }
}
