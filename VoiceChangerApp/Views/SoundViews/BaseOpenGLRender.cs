using SharpGL;
using SharpGL.WPF;
using System.Windows.Controls;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Views.SoundViews
{
    public abstract class BaseOpenGLRender : UserControl, IEnable, IRenderable
    {
        private bool _isNeedRedraw;

        public BaseOpenGLRender()
        {
            LayoutUpdated += BaseOpenGLRender_LayoutUpdated;
        }

        public abstract OpenGL OpenGL { get; }
        public abstract OpenGLControl OpenGLControl { get; }

        private void BaseOpenGLRender_LayoutUpdated(object sender, System.EventArgs e)
        {
            OpenGLControl.ForceRedraw();
        }

        public bool IsRendering()
        {
            return OpenGLControl.RenderContextType == RenderContextType.FBO;
        }

        public void RequestRedraw()
        {
            _isNeedRedraw = true;
        }
    }
}
