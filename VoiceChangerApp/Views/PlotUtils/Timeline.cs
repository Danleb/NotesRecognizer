using Microsoft.Extensions.Logging;
using Prism.Ioc;
using SharpGL;
using System;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public class Timeline : IEnable
    {
        private readonly OpenGL _gl;
        private readonly ILogger _logger;
        private uint _vbo = OpenGLUtils.NO_BUFFER;
        private uint _vao = OpenGLUtils.NO_BUFFER;
        private uint _program = OpenGLUtils.NO_PROGRAM;

        public Timeline(OpenGL openGL, OrthographicViewportMatrix viewport)
        {
            _gl = openGL;
            _logger = (ILogger)ContainerLocator.Container.Resolve(typeof(ILogger<SignalgramView>));
            Viewport = viewport;
            Initialize();
        }

        public bool IsEnabled { get; set; }
        public OrthographicViewportMatrix Viewport { get; set; }

        public void Draw(float time)
        {

        }

        private void Initialize()
        {
            InitializeProgram();
            InitializeBuffers();
        }

        private void InitializeProgram()
        {
            if (_program != OpenGLUtils.NO_PROGRAM)
            {
                return;
            }

            try
            {
                _program = _gl.CompileProgram(AppResources.Image_vert, AppResources.AmplitudeSpectrogram_frag);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to compile Timeline program.");
            }
        }

        private void InitializeBuffers()
        {

        }
    }
}
