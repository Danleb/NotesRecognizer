using SharpGL;
using SharpGL.WPF;
using System;
using System.Numerics;
using System.Windows.Media;
using VoiceChangerApp.Utils;
using AppResources = VoiceChangerApp.Resources;

namespace VoiceChangerApp.Views.SoundViews
{
    public class CoordinateGridDrawer : IRenderable
    {
        private const int SubSegmentsInUnit = 10;
        private readonly OpenGLControl _openGLControl;
        private readonly OpenGL _gl;
        private readonly uint[] _temp = new uint[5];
        private readonly uint _program;

        private uint _vbo;
        private uint _vao;
        private float[] _linesCoordinates;
        private int _mvpMatrixLocation;
        private int _verticesCount;

        public CoordinateGridDrawer(OpenGLControl openGLControl)
        {
            _openGLControl = openGLControl;
            _gl = openGLControl.OpenGL;
            _program = _gl.CompileProgram(AppResources.CoordinateGrid_vert, AppResources.CoordinateGrid_frag);
            _mvpMatrixLocation = _gl.GetUniformLocation(_program, "MVP");
            InitializeBuffers();
        }

        public OrthographicViewportMatrix Viewport { get; set; }

        public bool ShowTickLabels { get; set; } = true;

        public Color GridColor { get; set; } = Color.FromRgb(255, 255, 255);

        public Color TickLabelColor { get; set; } = Color.FromRgb(255, 255, 255);

        public float TickLabelFontSize { get; set; } = 20;

        public bool IsInitialized()
        {
            return _program != OpenGLUtils.NO_PROGRAM &&
                _vbo != OpenGLUtils.NO_BUFFER;
        }

        public void InitializeBuffers()
        {
            if (IsInitialized())
            {
                return;
            }

            _gl.GenBuffers(1, _temp);
            _vbo = _temp[0];
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);

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

        public void InitializeGrid(float duration)
        {
            InitializeBuffers();

            var linesCount = ((int)duration + 1) * SubSegmentsInUnit + 1;
            _verticesCount = (linesCount + 1) * 2;
            _linesCoordinates = new float[_verticesCount * 2];
            for (int i = 0; i < linesCount; i++)
            {
                var yValue = 0.15f;
                if (i % SubSegmentsInUnit == 0)
                {
                    yValue *= 3;
                }

                _linesCoordinates[i * 4] = (float)i / SubSegmentsInUnit;
                _linesCoordinates[i * 4 + 1] = -yValue;

                _linesCoordinates[i * 4 + 2] = (float)i / SubSegmentsInUnit;
                _linesCoordinates[i * 4 + 3] = yValue;
            }

            //x axis line
            _linesCoordinates[linesCount * 4] = 0;
            _linesCoordinates[linesCount * 4 + 1] = 0;
            _linesCoordinates[linesCount * 4 + 2] = duration;
            _linesCoordinates[linesCount * 4 + 3] = 0;

            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, _vbo);
            _gl.BufferData(OpenGL.GL_ARRAY_BUFFER, _linesCoordinates, OpenGL.GL_STATIC_DRAW);
            _gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGLUtils.NO_BUFFER);
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

            DrawTickLabels();
        }

        public void RequestRedraw()
        {

        }

        public void OnViewportChanged()
        {



        }

        private void DrawTickLabels()
        {
            if (!ShowTickLabels)
            {
                return;
            }

            int timeStart = (int)Math.Ceiling(Viewport.Left);
            int timeEnd = (int)Math.Floor(Viewport.Right);
            for (int time = timeStart; time <= timeEnd; time++)
            {
                var faceName = $"TickLabel #{time}";
                var position = new Vector2(time, 0);
                var projectedX = Viewport.Transform(position).X;
                var x = (int)Math.Round(((projectedX + 1.0) / 2.0) * _openGLControl.RenderSize.Width);
                _gl.DrawText(x, 10, 1, 0, 1, faceName, TickLabelFontSize, time.ToString());
            }
        }
    }
}
