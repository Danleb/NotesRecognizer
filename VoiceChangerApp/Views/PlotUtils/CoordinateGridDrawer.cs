using SharpGL;
using System;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public class CoordinateGridDrawer : IRenderable
    {
        private const int LinesCount = 100;
        private readonly OpenGL _gl;
        private readonly uint[] _temp = new uint[5];
        private readonly uint _program;
        private uint _vbo;
        private uint _vao;
        private float[] _linesCoordinates;
        private int _mvpMatrixLocation;
        private int _verticesCount;

        public CoordinateGridDrawer(OpenGL openGL)
        {
            _gl = openGL;
            _program = _gl.CompileProgram(AppResources.CoordinateGrid_vert, AppResources.CoordinateGrid_frag);
            _mvpMatrixLocation = _gl.GetUniformLocation(_program, "MVP");
            InitializeDynamicData();
        }

        public OrthographicViewportMatrix Viewport { get; set; }

        public bool IsInitialized()
        {
            return _program != OpenGLUtils.NO_PROGRAM &&
                _vbo != OpenGLUtils.NO_BUFFER;
        }

        public void InitializeDynamicData()
        {
            if (IsInitialized())
            {
                return;
            }

            _verticesCount = (LinesCount + 1) * 2;
            _linesCoordinates = new float[_verticesCount * 2];
            for (int i = 0; i < LinesCount; i++)
            {
                var yValue = 0.15f;
                if (i % 10 == 0)
                {
                    yValue *= 3;
                }

                _linesCoordinates[i * 4] = i / 10.0f;
                _linesCoordinates[i * 4 + 1] = -yValue;

                _linesCoordinates[i * 4 + 2] = i / 10.0f;
                _linesCoordinates[i * 4 + 3] = yValue;
            }

            _linesCoordinates[LinesCount * 4] = 0;
            _linesCoordinates[LinesCount * 4 + 1] = 0;
            _linesCoordinates[LinesCount * 4 + 2] = 100;
            _linesCoordinates[LinesCount * 4 + 3] = 0;

            _gl.GenBuffers(1, _temp);
            _vbo = _temp[0];
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);
            _gl.BufferData(OpenGL.GL_ARRAY_BUFFER, _linesCoordinates, OpenGL.GL_STATIC_DRAW);

            _gl.GenVertexArrays(1, _temp);
            _vao = _temp[0];
            _gl.BindVertexArray(_vao);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);

            int positionAttribute = _gl.GetAttribLocation(_program, "vertexPosition");
            _gl.VertexAttribPointer((uint)positionAttribute, 2, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
            _gl.EnableVertexAttribArray((uint)positionAttribute);

            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);
        }

        public void Render()
        {
            if (!IsInitialized())
            {
                return;
            }

            _gl.UseProgram(_program);
            _gl.UniformMatrix4(_mvpMatrixLocation, 1, false, Viewport.Matrix);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);
            _gl.BindVertexArray(_vao);

            var lineWidth = 3.0f;
            _gl.LineWidth(lineWidth);
            _gl.DrawArrays(OpenGL.GL_LINES, 0, _verticesCount);

            _gl.UseProgram(OpenGLUtils.NO_PROGRAM);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
            _gl.BindVertexArray(OpenGLUtils.NO_BUFFER);
        }

        public void RequestRedraw()
        {

        }

        public void OnViewportChanged()
        {



        }
    }
}
