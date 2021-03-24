using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public partial class Spectrum2dRenderView : UserControl, IRenderable
    {
        public static readonly DependencyProperty SpectrumSliceProperty = DependencyProperty.Register(
            nameof(SpectrumSlice),
            typeof(SpectrumSlice),
            typeof(Spectrum2dRenderView),
            new UIPropertyMetadata(null));

        private static readonly float[] QuadVertexBuffer =
        {
            -1.0f, -1.0f, 0, 0,
            1.0f, -1.0f, 1, 0,
            -1.0f, 1.0f, 0, 1,

            1.0f, 1.0f, 1, 1,
            -1.0f, 1.0f, 0, 1,
            1.0f, -1.0f, 1, 0,
        };

        private readonly ILogger _logger;
        private readonly uint[] _bufferTemp = new uint[10];
        private readonly BoundSquare _boundSquare = new(-1.0f, 1.0f, -1.0f, 1.0f);
        private OpenGL _gl;
        private uint _program = OpenGLUtils.NO_PROGRAM;
        private uint _vbo = OpenGLUtils.NO_BUFFER;
        private uint _vao = OpenGLUtils.NO_BUFFER;
        private uint _amplitudeBuffer = OpenGLUtils.NO_BUFFER;
        private int _startFrequencyElementIndex;
        private int _startFrequencyElementIndexLocation = -1;
        private int _endFrequencyElementIndex;
        private int _endFrequencyElementIndexLocation = -1;
        private int _minAmplitudeLocation = -1;
        private int _maxAmplitudeLocation = -1;
        private bool _isNeedRedraw = false;
        private OrthographicViewportMatrix _viewport;
        private PlotNavigator _navigator;

        public Spectrum2dRenderView()
        {
            InitializeComponent();
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<SignalgramView>));

            OpenGLControl.RenderContextType = RenderContextType.FBO;
            OpenGLControl.RenderTrigger = RenderTrigger.Manual;
            _navigator = new PlotNavigator(OpenGLControl, _viewport, _boundSquare, this);
            _gl = OpenGLControl.OpenGL;
        }

        [Bindable(true)]
        public SpectrumSlice SpectrumSlice
        {
            get => (SpectrumSlice)GetValue(SpectrumSliceProperty);
            set
            {
                SetValue(SpectrumSliceProperty, value);
                InitializeSpectrumSliceBuffer();
            }
        }

        #region

        public void RequestRedraw()
        {
            _isNeedRedraw = true;
            //OpenGLControl.DoRender();
        }

        private void InitializeSpectrumSliceBuffer()
        {
            if (SpectrumSlice == null)
            {
                return;
            }

            if (_amplitudeBuffer != OpenGLUtils.NO_BUFFER)
            {
                _bufferTemp[0] = _amplitudeBuffer;
                _gl.DeleteBuffers(1, _bufferTemp);
                _amplitudeBuffer = OpenGLUtils.NO_BUFFER;
            }

            _gl.GenBuffers(1, _bufferTemp);
            _amplitudeBuffer = _bufferTemp[0];
            _gl.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, _amplitudeBuffer);
            float[] amplitudes = new float[SpectrumSlice.Datas.Count];
            for (int i = 0; i < amplitudes.Length; i++)
            {
                amplitudes[i] = SpectrumSlice.Datas[i].Amplitude;
            }
            _gl.BufferData(OpenGL.GL_SHADER_STORAGE_BUFFER, amplitudes, OpenGL.GL_STATIC_DRAW);
            _gl.BindBufferBase(OpenGL.GL_SHADER_STORAGE_BUFFER, 0, _amplitudeBuffer);
            _gl.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, OpenGLUtils.NO_BUFFER);
        }

        private bool InitializeProgram()
        {
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return true;
            }

            try
            {
                _program = _gl.CompileProgram(AppResources.Image_vert, AppResources.AmplitudeSpectrogram_frag);
                _startFrequencyElementIndexLocation = _gl.GetUniformLocation(_program, "startFrequencyElementIndex");
                _endFrequencyElementIndexLocation = _gl.GetUniformLocation(_program, "endFrequencyElementIndex");
                _minAmplitudeLocation = _gl.GetUniformLocation(_program, "minAmplitude");
                _maxAmplitudeLocation = _gl.GetUniformLocation(_program, "maxAmplitude");
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to compile SignalgramView program.");
                return false;
            }
        }

        private bool CheckAndInitializeStaticData(OpenGL gl)
        {
            //_gl = gl;
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return true;
            }

            if (!InitializeProgram())
            {
                return false;
            }

            if (_vbo != OpenGLUtils.NO_BUFFER && _vao != OpenGLUtils.NO_BUFFER)
            {
                return true;
            }

            _gl.GenBuffers(1, _bufferTemp);
            _vbo = _bufferTemp[0];
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);
            _gl.BufferData(OpenGL.GL_ARRAY_BUFFER, QuadVertexBuffer, OpenGL.GL_STATIC_DRAW);

            _gl.GenVertexArrays(1, _bufferTemp);
            _vao = _bufferTemp[0];
            _gl.BindVertexArray(_vao);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);

            var stride = sizeof(float) * 4;

            int positionAttribute = _gl.GetAttribLocation(_program, "position");
            _gl.VertexAttribPointer((uint)positionAttribute, 2, OpenGL.GL_FLOAT, false, stride, IntPtr.Zero);
            _gl.EnableVertexAttribArray((uint)positionAttribute);

            int texCoordAttribute = _gl.GetAttribLocation(_program, "texCoord");
            _gl.VertexAttribPointer((uint)texCoordAttribute, 2, OpenGL.GL_FLOAT, false, stride, IntPtr.Zero);
            _gl.EnableVertexAttribArray((uint)texCoordAttribute);

            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);

            return true;
        }

        private bool IsDynamicDataInitialized()
        {
            return _amplitudeBuffer != OpenGLUtils.NO_BUFFER;
        }

        private bool IsRendering()
        {
            return true;
        }

        #endregion

        #region Drawing  callbacks

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            if (!IsRendering())
            {
                return;
            }
            //if (!_isNeedRedraw)
            //{
            //    return;
            //}
            if (!CheckAndInitializeStaticData(args.OpenGL))
            {
                return;
            }
            if (!IsDynamicDataInitialized())
            {
                return;
            }
            _isNeedRedraw = false;

            _gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            _gl.UseProgram(_program);
            _gl.Uniform1(_startFrequencyElementIndexLocation, _startFrequencyElementIndex);
            _gl.Uniform1(_endFrequencyElementIndexLocation, _endFrequencyElementIndex);
            _gl.Uniform1(_minAmplitudeLocation, SpectrumSlice.MinAmplitude);
            _gl.Uniform1(_maxAmplitudeLocation, SpectrumSlice.MaxAmplitude);
            _gl.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, _amplitudeBuffer);

            _gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 2);

            _gl.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.UseProgram(OpenGLUtils.NO_PROGRAM);
        }

        private void OpenGLControl_Resized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            RequestRedraw();
        }

        #endregion
    }
}
