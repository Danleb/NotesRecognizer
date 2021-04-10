using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using SharpGL.WPF;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public partial class Spectrum2dRenderView : BaseOpenGLRender
    {
        public static readonly DependencyProperty SpectrumSliceProperty = DependencyProperty.Register(
            nameof(SpectrumSlice),
            typeof(SpectrumSlice),
            typeof(Spectrum2dRenderView),
            new UIPropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                var view = (Spectrum2dRenderView)d;
                view.InitializeSpectrumSliceBuffer();
            }));

        private readonly BoundSquare _boundSquare = new(-1.0f, 1.0f, -1.0f, 1.0f);
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
        private int _mvpMatrixLocation = -1;
        private int _mousePositionLocation = -1;
        private int _valuesInPixelLocation = -1;
        private OrthographicViewportMatrix _viewport;
        private PlotNavigator _navigator;

        public Spectrum2dRenderView()
        {
            InitializeComponent();
            UpdateRenderingContext();
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<SignalgramView>));
            SelectedValueSignContainer.Visibility = Visibility.Hidden;
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

        public override OpenGL GL => MainOpenGLControl.OpenGL;

        public override OpenGLControl OpenGLControl => MainOpenGLControl;

        private void InitializeSpectrumSliceBuffer()
        {
            if (SpectrumSlice == null)
            {
                return;
            }

            if (_amplitudeBuffer != OpenGLUtils.NO_BUFFER)
            {
                _bufferTemp[0] = _amplitudeBuffer;
                GL.DeleteBuffers(1, _bufferTemp);
                _amplitudeBuffer = OpenGLUtils.NO_BUFFER;
            }

            GL.GenBuffers(1, _bufferTemp);
            _amplitudeBuffer = _bufferTemp[0];
            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, _amplitudeBuffer);
            float[] amplitudes = new float[SpectrumSlice.Datas.Count];
            for (int i = 0; i < amplitudes.Length; i++)
            {
                amplitudes[i] = SpectrumSlice.Datas[i].Amplitude;
            }
            GL.BufferData(OpenGL.GL_SHADER_STORAGE_BUFFER, amplitudes, OpenGL.GL_STATIC_DRAW);
            GL.BindBufferBase(OpenGL.GL_SHADER_STORAGE_BUFFER, 0, _amplitudeBuffer);
            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, OpenGLUtils.NO_BUFFER);

            _startFrequencyElementIndex = 0;
            _endFrequencyElementIndex = SpectrumSlice.FrequencyDataCount - 1;

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

        private bool InitializeProgram()
        {
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return true;
            }

            try
            {
                _program = GL.CompileProgram(AppResources.Image_vert, AppResources.AmplitudeSpectrogram_frag);
                _startFrequencyElementIndexLocation = GL.GetUniformLocation(_program, "startFrequencyElementIndex");
                _endFrequencyElementIndexLocation = GL.GetUniformLocation(_program, "endFrequencyElementIndex");
                _minAmplitudeLocation = GL.GetUniformLocation(_program, "minAmplitude");
                _maxAmplitudeLocation = GL.GetUniformLocation(_program, "maxAmplitude");
                _mvpMatrixLocation = GL.GetUniformLocation(_program, "MVP");
                _mousePositionLocation = GL.GetUniformLocation(_program, "mouseUV");
                _valuesInPixelLocation = GL.GetUniformLocation(_program, "valuesInPixel");
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to compile SignalgramView program.");
                return false;
            }
        }

        private bool CheckAndInitializeStaticData()
        {
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

            return true;
        }

        private bool IsDynamicDataInitialized()
        {
            return _amplitudeBuffer != OpenGLUtils.NO_BUFFER;
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            if (!IsRendering())
            {
                return;
            }
            //if (!_isNeedRedraw)
            //{
            //    return;
            //}
            if (!CheckAndInitializeStaticData())
            {
                return;
            }

            if (!IsDynamicDataInitialized())
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

            if (OpenGLControl.IsMouseOver)
            {
                var mouse = Mouse.GetPosition(OpenGLControl);
                var pointerScreenCube = new Vector2((float)(mouse.X / OpenGLControl.ActualWidth) * 2 - 1.0f, (float)(1.0f - mouse.Y / OpenGLControl.ActualHeight) * 2 - 1.0f);
                var pointerCamera = _viewport.InverseTransform(pointerScreenCube);
                float pointerU = (float)(pointerCamera.X + 1) / 2.0f;
                float pointerV = (float)(pointerCamera.Y + 1) / 2.0f;
                GL.Uniform2(_mousePositionLocation, pointerU, pointerV);
            }
            else
            {
                GL.Uniform2(_mousePositionLocation, -10.0f, -10.0f);
            }

            GL.Uniform1(_startFrequencyElementIndexLocation, _startFrequencyElementIndex);
            GL.Uniform1(_endFrequencyElementIndexLocation, _endFrequencyElementIndex);
            GL.Uniform1(_minAmplitudeLocation, SpectrumSlice.MinAmplitude);
            GL.Uniform1(_maxAmplitudeLocation, SpectrumSlice.MaxAmplitude);

            var visibilityRatio = _viewport.Width / 2.0f;
            var valuesInScreen = SpectrumSlice.FrequencyDataCount * visibilityRatio;
            var valuesInPixel = (int)Math.Ceiling(valuesInScreen / OpenGLControl.RenderSize.Width);
            GL.Uniform1(_valuesInPixelLocation, valuesInPixel);

            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, _amplitudeBuffer);

            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);

            GL.BindBuffer(OpenGL.GL_SHADER_STORAGE_BUFFER, OpenGLUtils.NO_BUFFER);
            GL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            GL.BindVertexArray(OpenGLUtils.NO_BUFFER);
            GL.UseProgram(OpenGLUtils.NO_PROGRAM);
        }

        //todo put to base
        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            RequestRedraw();
        }

        private void MainOpenGLControl_MouseLeave(object sender, MouseEventArgs e)
        {
            SelectedValueSignContainer.Visibility = Visibility.Hidden;
        }

        private void MainOpenGLControl_MouseEnter(object sender, MouseEventArgs e)
        {
            SelectedValueSignContainer.Visibility = Visibility.Visible;
        }

        private void MainOpenGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(OpenGLControl);
            Canvas.SetLeft(SelectedValueSignContainer, position.X - SelectedValueSignContainer.ActualWidth);
            Canvas.SetTop(SelectedValueSignContainer, position.Y - SelectedValueSignContainer.ActualHeight);
        }
    }
}
