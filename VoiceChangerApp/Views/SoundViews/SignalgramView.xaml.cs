using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private ILogger _logger;
        private OpenGL _gl;
        private uint[] _bufferTemp = new uint[10];
        private int[] _bufferTempInt = new int[10];
        private float[] _points;
        private uint _linesVBO = OpenGLUtils.NO_BUFFER;
        private uint _linesVAO = OpenGLUtils.NO_BUFFER;
        private uint _program = OpenGLUtils.NO_PROGRAM;
        private AudioContainer _containerCached;
        private OrthographicProjection _projection = new OrthographicProjection();
        Stopwatch sw = Stopwatch.StartNew();

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
            //RenderContextType.FBO
            //OpenGLControl.RenderContextType = isSelected ? RenderContextType.NativeWindow : RenderContextType.HiddenWindow;
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
                InitializeBuffers();
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
        public float ScaleSpeed { get; set; } = 0.1f;


        public void ResetView()
        {

        }

        private void InitializeBuffers()
        {
            if (_containerCached == AudioContainer)
            {
                return;
            }

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
            _points[0] = -1.0f;
            _points[1] = -1.0f;
            _points[2] = 1.0f;
            _points[3] = 1.0f;

            _gl.GenBuffers(1, _bufferTemp);
            _linesVBO = _bufferTemp[0];
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _linesVBO);
            _gl.BufferData(OpenGL.GL_ARRAY_BUFFER, _points, OpenGL.GL_STATIC_DRAW);

            _gl.GenVertexArrays(1, _bufferTemp);
            _linesVAO = _bufferTemp[0];
            _gl.BindVertexArray(_linesVAO);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _linesVBO);

            int positionAttribute = _gl.GetAttribLocation(_program, "position");
            _gl.VertexAttribPointer((uint)positionAttribute, 2, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
            _gl.EnableVertexAttribArray((uint)positionAttribute);



            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);
        }

        private void OpenGLControl_Initialized(object sender, System.EventArgs e)
        {
            _gl = OpenGLControl.OpenGL;


        }

        private void InitializeProgram()
        {
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return;
            }
            try
            {
                //var sourceString = Encoding.ASCII.GetString(AppResources.Pass_vert);
                //var sourceString2 = Encoding.ASCII.GetString(AppResources.Signalgram_frag);
                //ShaderProgram shaderProgram = new ShaderProgram();
                //shaderProgram.Create(_gl, sourceString, sourceString2, new System.Collections.Generic.Dictionary<uint, string>());
                _program = _gl.CompileProgram(AppResources.Pass_vert, AppResources.Signalgram_frag);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to compile SignalgramView program.");
            }
        }

        private void OpenGLControl_Resized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {

        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            _gl = args.OpenGL;

            if (sw.ElapsedMilliseconds > 5000)
            {
                InitializeProgram();
                InitializeBuffers();
            }

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
                _gl.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);
            }

            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            _gl.MatrixMode(OpenGL.GL_PROJECTION);
            //_gl.LoadIdentity();
            _gl.LoadMatrix(_projection.Matrix);
            //set matrix
            _gl.UseProgram(_program);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _linesVBO);
            _gl.BindVertexArray(_linesVAO);

            _gl.LineWidth(LineWidth);
            _gl.DrawArrays(OpenGL.GL_LINE_STRIP, 0, 100);//AudioContainer.ValuesCount

            _gl.UseProgram(OpenGLUtils.NO_PROGRAM);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);
        }

        private bool IsGLInitialized()
        {
            return _program != OpenGLUtils.NO_PROGRAM && _linesVBO != OpenGLUtils.NO_BUFFER;
        }

        private void OpenGLControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {

            }
            else
            {

            }

            Scale += ScaleSpeed * e.Delta;
            _projection.Right = Scale;
            _projection.UpdateMatrix();
        }

        private void OpenGLControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
    }
}
