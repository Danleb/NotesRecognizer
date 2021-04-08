using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using SharpGL.WPF;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using VoiceChanger.FormatParser;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public partial class SignalgramView : BaseOpenGLRender
    {
        public static readonly DependencyProperty AudioContainerProperty = DependencyProperty.Register(
            nameof(AudioContainer),
            typeof(AudioContainer),
            typeof(SignalgramView),
            new UIPropertyMetadata(null));
        public static readonly DependencyProperty SoundViewPositionProperty = DependencyProperty.Register(
            nameof(SoundViewPosition),
            typeof(SoundViewPosition),
            typeof(SignalgramView),
            new UIPropertyMetadata(null));
        public static readonly DependencyProperty LineWidthProperty = DependencyProperty.Register(
            nameof(LineWidth),
            typeof(float),
            typeof(SignalgramView),
            new UIPropertyMetadata(5.0f));
        public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register(
            nameof(LineColor),
            typeof(Color),
            typeof(SignalgramView),
            new UIPropertyMetadata(Color.White));

        private const float PlotYAxisUpscale = 3.0f;
        private OpenGL _gl;
        private float[] _points;
        private uint _linesVBO = OpenGLUtils.NO_BUFFER;
        private uint _linesVAO = OpenGLUtils.NO_BUFFER;
        private uint _program = OpenGLUtils.NO_PROGRAM;
        private AudioContainer _containerCached;
        private OrthographicViewportMatrix _viewport = new();
        private int _mvpMatrixLocation;
        private BoundSquare _boundSquare;
        private PlotNavigator _plotNavigator;
        private float[] _colorValues = new float[4];
        private CoordinateGridDrawer _coordinateGridDrawer;

        public SignalgramView()
        {
            InitializeComponent();
            UpdateRenderingContext();
            _gl = OpenGLControl.OpenGL;
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<SignalgramView>));
        }

        [Bindable(true)]
        public AudioContainer AudioContainer
        {
            get => (AudioContainer)GetValue(AudioContainerProperty);
            set
            {
                SetValue(AudioContainerProperty, value);
                CheckAndInitAudioData();
            }
        }

        [Bindable(true)]
        public SoundViewPosition SoundViewPosition
        {
            get => (SoundViewPosition)GetValue(SoundViewPositionProperty);
            set => SetValue(SoundViewPositionProperty, value);
        }

        [Bindable(true)]
        public float LineWidth
        {
            get => (float)GetValue(LineWidthProperty);
            set => SetValue(LineWidthProperty, value);
        }

        [Bindable(true)]
        public Color LineColor
        {
            get => (Color)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        public override OpenGL GL => MainOpenGLControl.OpenGL;
        public override OpenGLControl OpenGLControl => MainOpenGLControl;

        public void ResetView()
        {
            _viewport.Reset();
        }

        private void CheckAndInitAudioData()
        {
            if (_containerCached == AudioContainer)
            {
                return;
            }
            _containerCached = AudioContainer;

            _logger.LogDebug("Initializing SignalgramView.");
            if (_linesVBO != OpenGLUtils.NO_BUFFER)
            {
                _bufferTemp[0] = _linesVBO;
                _gl.DeleteBuffers(1, _bufferTemp);

                _bufferTemp[0] = _linesVAO;
                _gl.DeleteVertexArrays(1, _bufferTemp);
            }

            _points = new float[AudioContainer.Samples.Length * 2];

            //Parallel.ForEach(AudioContainer.Data, (q, w, e) =>
            //{

            //});

            var signalDuration = 1.0f / AudioContainer.SampleRate;

            for (int i = 0; i < AudioContainer.Samples.Length; i++)
            {
                _points[i * 2] = i * signalDuration;
                _points[i * 2 + 1] = AudioContainer.Samples[i];
            }

            _gl.GenBuffers(1, _bufferTemp);
            _linesVBO = _bufferTemp[0];
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _linesVBO);
            _gl.BufferData(OpenGL.GL_ARRAY_BUFFER, _points, OpenGL.GL_STATIC_DRAW);

            _gl.GenVertexArrays(1, _bufferTemp);
            _linesVAO = _bufferTemp[0];
            _gl.BindVertexArray(_linesVAO);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _linesVBO);

            int positionAttribute = _gl.GetAttribLocation(_program, "vertexPosition");
            _gl.VertexAttribPointer((uint)positionAttribute, 2, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
            _gl.EnableVertexAttribArray((uint)positionAttribute);

            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);

            _boundSquare = new BoundSquare(0, AudioContainer.Duration, AudioContainer.Min * PlotYAxisUpscale, AudioContainer.Max * PlotYAxisUpscale);

            _viewport = new();
            _viewport.Bottom = _boundSquare.Bottom;
            _viewport.Top = _boundSquare.Top;
            _viewport.Left = _boundSquare.Left;
            _viewport.Right = _boundSquare.Right;
            //_viewport.Right = 0.5f;
            _viewport.UpdateMatrix();

            _coordinateGridDrawer.Viewport = _viewport;
            _coordinateGridDrawer.InitializeGrid(AudioContainer.Duration);

            if (_plotNavigator == null)
            {
                _plotNavigator = new PlotNavigator(OpenGLControl, _viewport, _boundSquare, this);
            }
            else
            {
                _plotNavigator.Viewport = _viewport;
                _plotNavigator.BoundSquare = _boundSquare;
            }
        }

        private void CheckAndInitProgram()
        {
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return;
            }
            try
            {
                _program = _gl.CompileProgram(AppResources.SimpleMatrix_vert, AppResources.Signalgram_frag);
                _mvpMatrixLocation = _gl.GetUniformLocation(_program, "MVP");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to compile SignalgramView program.");
            }
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            RequestRedraw();
        }

        private bool IsGLInitialized()
        {
            return _program != OpenGLUtils.NO_PROGRAM && _linesVBO != OpenGLUtils.NO_BUFFER;
        }

        private void CheckAndInitStaticData()
        {
            CheckAndInitProgram();
            if (_coordinateGridDrawer == null)
            {
                _coordinateGridDrawer = new CoordinateGridDrawer(OpenGLControl);
            }
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            if (!IsRendering())
            {
                return;
            }
            //if (!_isRedrawNeeded)
            //{
            //    return;
            //}

            CheckAndInitStaticData();
            CheckAndInitAudioData();

            if (!IsGLInitialized())
            {
                return;
            }

            //_isRedrawNeeded = false;

            if (AudioContainer == null)
            {
                _gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            }
            else
            {
                _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            }

            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            _gl.UseProgram(_program);
            _gl.UniformMatrix4(_mvpMatrixLocation, 1, false, _viewport.Matrix);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _linesVBO);
            _gl.BindVertexArray(_linesVAO);

            //todo put out
            int lineColorLocation = _gl.GetUniformLocation(_program, "color");
            _colorValues[0] = LineColor.R;
            _colorValues[1] = LineColor.G;
            _colorValues[2] = LineColor.B;
            _colorValues[3] = LineColor.A;
            _gl.Uniform4(lineColorLocation, 1, _colorValues);
            //

            _gl.LineWidth(LineWidth);
            _gl.DrawArrays(OpenGL.GL_LINE_STRIP, 0, AudioContainer.SamplesCount);

            _gl.UseProgram(OpenGLUtils.NO_PROGRAM);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);

            _coordinateGridDrawer.Render();
        }
    }
}
