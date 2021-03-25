using SharpGL;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public class CoordinateGridDrawer : IRenderable
    {
        private readonly OpenGL _openGL;
        private readonly uint[] _temp = new uint[5];
        private readonly uint _program;
        private readonly uint _vbo;
        private readonly uint _vao;

        public CoordinateGridDrawer(OpenGL openGL)
        {
            _openGL = openGL;
            _program = _openGL.CompileProgram(AppResources.CoordinateGrid_vert, AppResources.CoordinateGrid_frag);
        }

        public OrthographicViewportMatrix Viewport { get; set; }

        public void Render()
        {
            _openGL.UseProgram(_program);


            //_openGL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);

            _openGL.UseProgram(OpenGLUtils.NO_PROGRAM);
        }

        public void RequestRedraw()
        {

        }
    }
}
