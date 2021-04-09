using System.Numerics;

namespace VoiceChangerApp.Views.SoundViews
{
    public class OrthographicViewportMatrix
    {
        private static void DoScale(float newScale, float screenRatio, float borderLeft, float width, ref float left, ref float right)
        {
            var oldWidth = width;
            var newWidth = oldWidth * newScale;
            var scrollCenter = left * (1.0f - screenRatio) + right * screenRatio;

            var leftShift = newWidth * screenRatio;
            left = scrollCenter - leftShift;

            if (left < borderLeft)
            {
                left = borderLeft;
                right = left + newWidth;
            }
            else
            {
                var rightShift = newWidth * (1.0f - screenRatio);
                right = scrollCenter + rightShift;
            }
        }

        public float[] Matrix = new float[16];

        private float _left;
        private float _right;
        private float _bottom;
        private float _top;
        private float _near;
        private float _far;

        public OrthographicViewportMatrix()
        {
            Reset();
            UpdateMatrix();
        }

        public float Top { get => _top; set => _top = value; }
        public float Bottom { get => _bottom; set => _bottom = value; }
        public float Left { get => _left; set => _left = value; }
        public float Right { get => _right; set => _right = value; }
        public float Far { get => _far; set => _far = value; }
        public float Near { get => _near; set => _near = value; }
        public float Width => Right - Left;
        public float Height => Top - Bottom;

        public float CenterX
        {
            get => (Left + Right) / 2.0f;
            set
            {
                var halfWidth = Width / 2.0f;
                Left = value - halfWidth;
                Right = value + halfWidth;
            }
        }

        public float CenterY
        {
            get => (Bottom + Top) / 2.0f;
            set
            {
                var halfHeight = Height / 2.0f;
                Bottom = value - halfHeight;
                Top = value + halfHeight;
            }
        }

        public void UpdateMatrix()
        {
            Matrix[0 * 4 + 0] = 2 / (Right - Left);
            Matrix[0 * 4 + 1] = 0;
            Matrix[0 * 4 + 2] = 0;
            Matrix[0 * 4 + 3] = 0;

            Matrix[1 * 4 + 0] = 0;
            Matrix[1 * 4 + 1] = 2 / (Top - Bottom);
            Matrix[1 * 4 + 2] = 0;
            Matrix[1 * 4 + 3] = 0;

            Matrix[2 * 4 + 0] = 0;
            Matrix[2 * 4 + 1] = 0;
            Matrix[2 * 4 + 2] = -2 / (Far - Near);
            Matrix[2 * 4 + 3] = 0;

            Matrix[3 * 4 + 0] = -(Right + Left) / (Right - Left);
            Matrix[3 * 4 + 1] = -(Top + Bottom) / (Top - Bottom);
            Matrix[3 * 4 + 2] = -(Far + Near) / (Far - Near);
            Matrix[3 * 4 + 3] = 1;
        }

        public void DoScaleX(float newScale, BoundSquare boundSquare, float screenRatio = 0.5f)
        {
            DoScale(newScale, screenRatio, boundSquare.Left, Width, ref _left, ref _right);
            UpdateMatrix();
        }

        public void DoScaleY(float newScale, BoundSquare boundSquare, float screenRatio = 0.5f)
        {
            DoScale(newScale, screenRatio, boundSquare.Bottom, Height, ref _bottom, ref _top);
            UpdateMatrix();
        }

        public void SetLeftPreservingWidth(float newLeft)
        {
            var width = Width;
            Left = newLeft;
            Right = newLeft + width;
        }

        public void SetRightPreservingWidth(float newRight)
        {
            var width = Width;
            Right = newRight;
            Left = newRight - width;
        }

        public void Reset()
        {
            _left = -1;
            _right = 1;
            _bottom = -1;
            _top = 1;
            _far = -1;
            _near = 1;
        }

        public Vector2 Transform(Vector2 v)
        {
            return new Vector2(
                Matrix[0] * v.X + Matrix[3 * 4 + 0],
                Matrix[5] * v.Y + Matrix[3 * 4 + 1]);
        }

        public Vector2 InverseTransform(Vector2 v)
        {
            return new Vector2(
                (v.X - Matrix[3 * 4 + 0]) / Matrix[0],
                (v.Y - Matrix[3 * 4 + 1]) / Matrix[5]);
        }
    }
}
