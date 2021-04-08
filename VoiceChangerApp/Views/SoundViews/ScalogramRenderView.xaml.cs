using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using SharpGL.WPF;
using System;
using System.ComponentModel;
using System.Windows;
using VoiceChanger.Scalogram;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public partial class ScalogramRenderView : BaseOpenGLRender
    {
        public static readonly DependencyProperty ScalogramContainerProperty = DependencyProperty.Register(
            nameof(ScalogramContainer),
            typeof(ScalogramContainer),
            typeof(ScalogramRenderView),
            new UIPropertyMetadata((d, e) =>
            {
                var view = (ScalogramRenderView)d;
                view.InitializeDynamicData();
            }));

        private uint _program = OpenGLUtils.NO_PROGRAM;
        private uint _vbo = OpenGLUtils.NO_BUFFER;
        private uint _vao = OpenGLUtils.NO_BUFFER;
        private uint _scalogramDataBuffer = OpenGLUtils.NO_BUFFER;
        private OrthographicViewportMatrix _viewport;
        private PlotNavigator _navigator;
        private readonly BoundSquare _boundSquare = new(-1.0f, 1.0f, -1.0f, 1.0f);
        private int _mvpMatrixLocation = -1;

        public ScalogramRenderView()
        {
            InitializeComponent();
            UpdateRenderingContext();
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<ScalogramRenderView>));
        }

        public override OpenGL GL => MainOpenGLControl.OpenGL;
        public override OpenGLControl OpenGLControl => MainOpenGLControl;

        [Bindable(true)]
        public ScalogramContainer ScalogramContainer
        {
            get => (ScalogramContainer)GetValue(ScalogramContainerProperty);
            set
            {
                SetValue(ScalogramContainerProperty, value);
                InitializeDynamicData();
            }
        }

        private void InitializeStaticData()
        {
            InitializeProgram();
        }

        private bool InitializeProgram()
        {
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return true;
            }

            try
            {
                _program = GL.CompileProgram(AppResources.Image_vert, AppResources.Scalogram_frag);
                _mvpMatrixLocation = GL.GetUniformLocation(_program, "MVP");
                return true;
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Failed to compile ScalogramRenderView program.");
                return false;
            }
        }

        private void InitializeDynamicData()
        {

        }

        private bool IsOpenglInitialized()
        {
            return _program != OpenGLUtils.NO_PROGRAM &&
                _vbo != OpenGLUtils.NO_BUFFER &&
                _vao != OpenGLUtils.NO_BUFFER &&
                _scalogramDataBuffer != OpenGLUtils.NO_BUFFER;
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            if (!IsRendering())
            {
                return;
            }
            InitializeStaticData();
            InitializeDynamicData();
            if (!IsOpenglInitialized())
            {
                return;
            }
            _isNeedRedraw = false;

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            GL.UseProgram(_program);
            GL.UniformMatrix4(_mvpMatrixLocation, 1, false, _viewport.Matrix);
            GL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);
            GL.BindVertexArray(_vao);

            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);

            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, OpenGLUtils.NO_BUFFER);
            GL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            GL.BindVertexArray(OpenGLUtils.NO_BUFFER);
            GL.UseProgram(OpenGLUtils.NO_PROGRAM);
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            RequestRedraw();
        }
    }
}
