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
        private int _startFrequencyElementIndexLocation = -1;
        private int _endFrequencyElementIndexLocation = -1;
        private int _startSignalIndexLocation = -1;
        private int _endSignalIndexLocation = -1;
        private int _signalsCountInRowLocation = -1;

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
            InitializeQuad();
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
                _startFrequencyElementIndexLocation = GL.GetUniformLocation(_program, "startFrequencyElementIndex");
                _endFrequencyElementIndexLocation = GL.GetUniformLocation(_program, "endFrequencyElementIndex");
                _startSignalIndexLocation = GL.GetUniformLocation(_program, "startSignalIndex");
                _endSignalIndexLocation = GL.GetUniformLocation(_program, "endSignalIndex");
                _signalsCountInRowLocation = GL.GetUniformLocation(_program, "signalsCountInRow");
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to compile ScalogramRenderView program.");
                return false;
            }
        }

        private void InitializeQuad()
        {
            if (_vbo != OpenGLUtils.NO_BUFFER && _vao != OpenGLUtils.NO_BUFFER)
            {
                return;
            }

            GL.GenBuffers(1, _bufferTemp);
            _vbo = _bufferTemp[0];
            GL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.QuadVertexBuffer, OpenGL.GL_STATIC_DRAW);

            GL.GenVertexArrays(1, _bufferTemp);
            _vao = _bufferTemp[0];
            GL.BindVertexArray(_vao);
            GL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);

            //2 float position + 2 float texCoord
            var stride = sizeof(float) * 4;

            int positionAttribute = GL.GetAttribLocation(_program, "position");
            GL.VertexAttribPointer((uint)positionAttribute, 2, OpenGL.GL_FLOAT, false, stride, IntPtr.Zero);
            GL.EnableVertexAttribArray((uint)positionAttribute);

            int texCoordAttribute = GL.GetAttribLocation(_program, "texCoord");
            GL.VertexAttribPointer((uint)texCoordAttribute, 2, OpenGL.GL_FLOAT, false, stride, new IntPtr(2 * sizeof(float)));
            GL.EnableVertexAttribArray((uint)texCoordAttribute);

            GL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            GL.BindVertexArray(OpenGLUtils.NO_BUFFER);
        }

        private void InitializeDynamicData()
        {
            if (_scalogramDataBuffer != OpenGLUtils.NO_BUFFER)
            {
                _bufferTemp[0] = _scalogramDataBuffer;
                GL.DeleteBuffers(1, _bufferTemp);
            }

            GL.GenBuffers(1, _bufferTemp);
            _scalogramDataBuffer = _bufferTemp[0];
            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, _scalogramDataBuffer);
            GL.BufferData(OpenGL.GL_SHADER_STORAGE_BUFFER, ScalogramContainer.ScalogramValues, OpenGL.GL_STATIC_DRAW);
            GL.BindBufferBase(OpenGL.GL_SHADER_STORAGE_BUFFER, 0, _scalogramDataBuffer);
            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, OpenGLUtils.NO_BUFFER);

            _viewport = new OrthographicViewportMatrix
            {
                Left = -1,
                Right = 1,
                Bottom = -1,
                Top = 1
            };

            if (_navigator == null)
            {
                _navigator = new PlotNavigator(OpenGLControl, _viewport, _boundSquare, this);
            }
            else
            {
                _navigator.Viewport = _viewport;
            }
        }

        private bool IsDrawingInitialized()
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
            if (!IsDrawingInitialized())
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

            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, _scalogramDataBuffer);

            GL.Uniform1(_startFrequencyElementIndexLocation, 0);
            GL.Uniform1(_endFrequencyElementIndexLocation, ScalogramContainer.FrequenciesCount);
            GL.Uniform1(_startSignalIndexLocation, 0);
            GL.Uniform1(_endSignalIndexLocation, ScalogramContainer.SignalsCount);
            GL.Uniform1(_signalsCountInRowLocation, ScalogramContainer.SignalsCount);

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
