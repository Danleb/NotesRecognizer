using SharpGL;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Views.SoundViews
{
    public class Timeline : IEnable
    {
        private readonly OpenGL _gl;

        public Timeline(OpenGL openGL, OrthographicViewportMatrix viewport)
        {
            _gl = openGL;
            Viewport = viewport;
            InitializeBuffers();
        }

        public bool IsEnabled { get; set; }
        public OrthographicViewportMatrix Viewport { get; set; }

        public void Draw(float time)
        {

        }

        private void InitializeBuffers()
        {

        }
    }
}
