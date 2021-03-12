using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using SharpGL.WPF;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VoiceChanger.FormatParser;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public partial class SignalgramView : UserControl
    {
        public static readonly DependencyProperty AudioContainerProperty = DependencyProperty.Register(
            nameof(AudioContainer),
            typeof(AudioContainer),
            typeof(SignalgramView),
            new UIPropertyMetadata(null));
        public static readonly DependencyProperty SoundViewPositionProperty = DependencyProperty.Register(
            nameof(SoundViewPositionProperty),
            typeof(SoundViewPosition),
            typeof(SignalgramView),
            new UIPropertyMetadata(null));
        public static readonly DependencyProperty LineWidthProperty = DependencyProperty.Register(
            nameof(LineWidthProperty),
            typeof(float),
            typeof(SignalgramView),
            new UIPropertyMetadata(5.0f));

        private readonly ILogger _logger;
        private readonly uint[] _bufferTemp = new uint[10];
        private OpenGL _gl;
        private float[] _points;
        private uint _linesVBO = OpenGLUtils.NO_BUFFER;
        private uint _linesVAO = OpenGLUtils.NO_BUFFER;
        private uint _program = OpenGLUtils.NO_PROGRAM;
        private AudioContainer _containerCached;
        private OrthographicProjection _projection = new();
        private int _mvpMatrixLocation;
        private DispatcherTimer _keyTimer;
        private bool _isMousePressed = false;
        private Point _mousePressedStart;

        public SignalgramView()
        {
            InitializeComponent();
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<SignalgramView>));

            //OpenGLControl.RenderContextType = RenderContextType.HiddenWindow;
            OpenGLControl.RenderContextType = RenderContextType.FBO;
            OpenGLControl.RenderTrigger = RenderTrigger.Manual;
        }

        public void SetSelected(bool isSelected)
        {
            OpenGLControl.RenderContextType = isSelected ? RenderContextType.FBO : RenderContextType.HiddenWindow;
            if (isSelected)
            {
                OpenGLControl.RenderTrigger = RenderTrigger.TimerBased;
            }
        }

        [Bindable(true)]
        public AudioContainer AudioContainer
        {
            get { return (AudioContainer)GetValue(AudioContainerProperty); }
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

        public float Scale { get; set; } = 1.0f;
        public float ScaleSpeed { get; set; } = 1.0f / 800.0f;


        public void ResetView()
        {

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

            _points = new float[AudioContainer.Data.Length * 2];

            //Parallel.ForEach(AudioContainer.Data, (q, w, e) =>
            //{

            //});

            var signalDuration = 1.0f / AudioContainer.SampleRate;

            for (int i = 0; i < AudioContainer.Data.Length; i++)
            {
                _points[i * 2] = i * signalDuration;
                _points[i * 2 + 1] = AudioContainer.Data[i];
            }
            //_points[0] = -1.0f;
            //_points[1] = -1.0f;
            //_points[2] = 1.0f;
            //_points[3] = 1.0f;

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

            _mvpMatrixLocation = _gl.GetUniformLocation(_program, "MVP");

            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);

            _projection = new();
            _projection.Top = AudioContainer.Max;
            _projection.Bottom = AudioContainer.Min;
            _projection.Left = 0;
            _projection.Right = 1;
            _projection.UpdateMatrix();
        }

        private void OpenGLControl_Initialized(object sender, System.EventArgs e)
        {
            _gl = OpenGLControl.OpenGL;


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
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to compile SignalgramView program.");
            }
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {

        }

        private bool IsRendering()
        {
            return OpenGLControl.RenderContextType == RenderContextType.FBO;
        }

        private bool IsGLInitialized()
        {
            return _program != OpenGLUtils.NO_PROGRAM && _linesVBO != OpenGLUtils.NO_BUFFER;
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            _gl = args.OpenGL;

            if (!IsRendering())
            {
                return;
            }

            CheckAndInitProgram();
            CheckAndInitAudioData();

            if (!IsGLInitialized())
            {
                return;
            }

            if (AudioContainer == null)
            {
                _gl.ClearColor(0.1f, 0.1f, 1.1f, 1.0f);
            }
            else
            {
                _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            }

            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            _gl.UseProgram(_program);
            _gl.UniformMatrix4(_mvpMatrixLocation, 1, false, _projection.Matrix);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _linesVBO);
            _gl.BindVertexArray(_linesVAO);

            _gl.LineWidth(LineWidth);
            _gl.DrawArrays(OpenGL.GL_LINE_STRIP, 0, AudioContainer.ValuesCount);

            _gl.UseProgram(OpenGLUtils.NO_PROGRAM);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);
        }

        private void OpenGLControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {

            }
            else
            {
                var diff = ScaleSpeed * e.Delta;
                _projection.ScaleX += diff;
                //_logger.LogInformation($"ScaleX={_projection.ScaleX} Diff={diff}");
            }

            _projection.UpdateMatrix();
        }

        private void OpenGLControl_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void OpenGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMousePressed)
            {
                return;
            }

            var delta = e.GetPosition(OpenGLControl) - _mousePressedStart;
            var deltaX = (float)delta.X / 300.0f;
            _projection.Right += deltaX;
            _projection.Left += deltaX;
            _projection.UpdateMatrix();
        }

        private void OpenGLControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = true;
            _mousePressedStart = e.GetPosition(OpenGLControl);
        }

        private void OpenGLControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = false;
        }
    }
}
