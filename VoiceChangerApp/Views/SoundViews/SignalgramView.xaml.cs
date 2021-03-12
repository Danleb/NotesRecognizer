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
        private OrthographicViewportMatrix _viewport = new();
        private int _mvpMatrixLocation;
        private DispatcherTimer _keyTimer;
        private bool _isMousePressed = false;
        private Point _moveStartMousePosition;
        private float _moveStartViewportCenterPosition;
        private bool _isRedrawNeeded = false;

        public SignalgramView()
        {
            InitializeComponent();
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<SignalgramView>));
            _keyTimer = new DispatcherTimer();
            _keyTimer.Interval = TimeSpan.FromSeconds(1.0f / 30);
            _keyTimer.Tick += KeyboardTimerTick;
            _keyTimer.Start();

            //OpenGLControl.RenderContextType = RenderContextType.HiddenWindow;
            OpenGLControl.RenderContextType = RenderContextType.FBO;
            OpenGLControl.RenderTrigger = RenderTrigger.Manual;
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
        public float MouseWheelScaleSpeed { get; set; } = 1.0f / 800.0f;
        public float KeyboardNavigationSpeed { get; set; } = 1.0f / 800.0f;
        private float TimePerPixel => _viewport.ScaledWidth / (float)OpenGLControl.ActualWidth;

        public virtual OpenGLControl GetOpenGLControl() => OpenGLControl;

        public void SetActiveRenderState(bool isSelected)
        {
            OpenGLControl.RenderContextType = isSelected ? RenderContextType.FBO : RenderContextType.HiddenWindow;
            if (isSelected)
            {
                OpenGLControl.RenderTrigger = RenderTrigger.TimerBased;
            }
        }

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

            _viewport = new();
            _viewport.Top = AudioContainer.Max;
            _viewport.Bottom = AudioContainer.Min;
            _viewport.Left = 0;
            _viewport.Right = 1;
            _viewport.UpdateMatrix();
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
            RequestRedraw();
        }

        private bool IsRendering()
        {
            return OpenGLControl.RenderContextType == RenderContextType.FBO;
        }

        private bool IsGLInitialized()
        {
            return _program != OpenGLUtils.NO_PROGRAM && _linesVBO != OpenGLUtils.NO_BUFFER;
        }

        private void RequestRedraw()
        {
            _isRedrawNeeded = true;
        }

        private void ClampValues()
        {
            _viewport.Left = MathF.Max(0, _viewport.Left);
            _viewport.Right = MathF.Min(AudioContainer.Duration, _viewport.Right);
            _viewport.Bottom = MathF.Max(AudioContainer.Min, _viewport.Bottom);
            _viewport.Top = MathF.Min(AudioContainer.Max, _viewport.Top);
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

            //if (!_isRedrawNeeded)
            //{
            //    return;
            //}
            //_isRedrawNeeded = false;

            ClampValues();

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

            _gl.LineWidth(LineWidth);
            _gl.DrawArrays(OpenGL.GL_LINE_STRIP, 0, AudioContainer.ValuesCount);

            _gl.UseProgram(OpenGLUtils.NO_PROGRAM);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);
        }

        #region Navigation

        private void OpenGLControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var diff = MouseWheelScaleSpeed * e.Delta;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _viewport.ScaleY += diff;
            }
            else
            {
                _viewport.ScaleX += diff;
            }

            _viewport.UpdateMatrix();
        }

        private void KeyboardTimerTick(object sender, EventArgs e)
        {
            Vector direction = new(0, 0);
            bool pressed = false;
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                pressed = true;
                direction.Y = -1;
            }
            if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
            {
                pressed = true;
                direction.Y = 1;
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                pressed = true;
                direction.X = -1;
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                pressed = true;
                direction.X = 1;
            }

            if (!pressed)
            {
                return;
            }

            var timeDelta = _keyTimer.Interval.TotalMilliseconds;
            direction.Normalize();
            var offset = direction * timeDelta * KeyboardNavigationSpeed;
            _viewport.Center += (float)offset.X;
            _viewport.UpdateMatrix();
            ClampValues();
            RequestRedraw();
        }

        private void OpenGLControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = true;
            _moveStartMousePosition = e.GetPosition(OpenGLControl);
            _moveStartViewportCenterPosition = _viewport.Center;
        }

        private void OpenGLControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMousePressed = false;
        }

        private void OpenGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                _isMousePressed = false;
            }
            if (!_isMousePressed)
            {
                return;
            }

            var delta = _moveStartMousePosition - e.GetPosition(OpenGLControl);
            var deltaX = (float)delta.X;
            var timeDelta = deltaX * TimePerPixel;
            _logger.LogDebug($"SoundView MouseMove DeltaX={deltaX} ControlWidth={OpenGLControl.ActualWidth} TimePerPixel={TimePerPixel} timeDelta={timeDelta}");
            _viewport.Center = _moveStartViewportCenterPosition + timeDelta;
            _viewport.UpdateMatrix();
        }

        private void OpenGLControl_KeyDown(object sender, KeyEventArgs e)
        {

        }

        #endregion
    }
}
