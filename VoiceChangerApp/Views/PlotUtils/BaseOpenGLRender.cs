using Microsoft.Extensions.Logging;
using SharpGL;
using SharpGL.WPF;
using System.Windows.Controls;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Views.SoundViews
{
    public abstract class BaseOpenGLRender : UserControl, IEnable, IRenderable
    {
        protected readonly uint[] _bufferTemp = new uint[10];
        protected ILogger _logger;
        protected bool _isNeedRedraw;

        public BaseOpenGLRender()
        {
            IsVisibleChanged += BaseOpenGLRender_IsVisibleChanged;
            Loaded += BaseOpenGLRender_Loaded;
        }

        private void BaseOpenGLRender_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LayoutUpdated += BaseOpenGLRender_LayoutUpdated;
        }

        public abstract OpenGL GL { get; }
        public abstract OpenGLControl OpenGLControl { get; }

        public bool IsRendering()
        {
            return OpenGLControl.RenderContextType == RenderContextType.FBO;
        }

        public void RequestRedraw()
        {
            _isNeedRedraw = true;
        }

        private void BaseOpenGLRender_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            UpdateRenderingContext();
        }

        private void BaseOpenGLRender_LayoutUpdated(object sender, System.EventArgs e)
        {
            LayoutUpdated -= BaseOpenGLRender_LayoutUpdated;
            OpenGLControl.ForceRedraw();
        }

        protected void UpdateRenderingContext()
        {
            OpenGLControl.RenderContextType = IsVisible ? RenderContextType.FBO : RenderContextType.HiddenWindow;
            OpenGLControl.RenderTrigger = IsVisible ? RenderTrigger.TimerBased : RenderTrigger.Manual;
        }
    }
}
