using SharpGL;
using System;
using System.Windows;

namespace VoiceChangerApp
{
    public partial class EditorWindow : Window
    {
        public EditorWindow()
        {
            InitializeComponent();
        }

        private void OpenGlControl_Initialized(object sender, EventArgs e)
        {

        }

        private void OpenGlControl_Resized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {

        }

        private void OpenGlControl_OpenGLDraw(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
            gl.ClearColor(0.1f, 1f, 0f, 1f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
        }
    }
}
