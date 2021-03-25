using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using SharpGL.WPF;
using System.ComponentModel;
using System.Windows;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Views.SoundViews
{
    public partial class Spectrum3dRenderView : BaseOpenGLRender
    {
        public static readonly DependencyProperty SpectrumContainerProperty = DependencyProperty.Register(
            nameof(SpectrumContainer),
            typeof(SpectrumContainer),
            typeof(Spectrum3dRenderView),
            new UIPropertyMetadata((d, e) =>
            {
                var view = (Spectrum3dRenderView)d;
                view.InitializeSpectrumContainerData();
            }));

        private uint _program = OpenGLUtils.NO_PROGRAM;
        private uint _vbo = OpenGLUtils.NO_BUFFER;
        private uint _vao = OpenGLUtils.NO_BUFFER;
        private uint _spectrumBuffer = OpenGLUtils.NO_BUFFER;
        private OrthographicViewportMatrix _viewport;
        private PlotNavigator _navigator;
        private readonly BoundSquare _boundSquare = new(-1.0f, 1.0f, -1.0f, 1.0f);

        public Spectrum3dRenderView()
        {
            InitializeComponent();
            UpdateRenderingContext();
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<Spectrum3dRenderView>));
        }

        public override OpenGL GL => MainOpenGLControl.OpenGL;
        public override OpenGLControl OpenGLControl => MainOpenGLControl;

        [Bindable(true)]
        public SpectrumContainer SpectrumContainer
        {
            get => (SpectrumContainer)GetValue(SpectrumContainerProperty);
            set
            {
                SetValue(SpectrumContainerProperty, value);
                InitializeSpectrumContainerData();
            }
        }

        private void InitStaticData()
        {
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return;
            }


        }

        private void InitializeSpectrumContainerData()
        {

        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            GL.UseProgram(_program);

            GL.UseProgram(OpenGLUtils.NO_PROGRAM);
            GL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {

        }
    }
}
