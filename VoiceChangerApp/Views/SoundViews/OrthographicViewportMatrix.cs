namespace VoiceChangerApp.Views.SoundViews
{
    public class OrthographicViewportMatrix
    {
        public float[] Matrix = new float[16];

        private float _top;
        private float _bottom;
        private float _left;
        private float _right;
        private float _far;
        private float _near;
        private float _scaleX;
        private float _scaleY;

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
        public float ScaleX { get => _scaleX; set => _scaleX = value; }
        public float ScaleY { get => _scaleY; set => _scaleY = value; }
        public float ScaledWidth => (Right - Left) * ScaleX;
        public float ScaledHeight => (Top - Bottom) * ScaleY;
        public float Width => Right - Left;
        public float Height => Top - Bottom;

        public float Center
        {
            get => (Left + Right) / 2.0f;
            set
            {
                var halfWidth = Width / 2.0f;
                Left = value - halfWidth;
                Right = value + halfWidth;
            }
        }

        public void UpdateMatrix()
        {
            Matrix[0 * 4 + 0] = (2 / (Right - Left)) * ScaleX;
            Matrix[0 * 4 + 1] = 0;
            Matrix[0 * 4 + 2] = 0;
            Matrix[0 * 4 + 3] = 0;

            Matrix[1 * 4 + 0] = 0;
            Matrix[1 * 4 + 1] = 2 / (Top - Bottom) * ScaleY;
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

        //public void ScaleX(float ratio)
        //{

        //}

        public void SetLeftPreservingWidth(float newLeft)
        {

        }

        public void SetRightPreservingWidth(float newRight)
        {

        }

        public void Reset()
        {
            _bottom = -1;
            _top = 1;
            _left = -1;
            _right = 1;
            _far = -1;
            _near = 1;
            _scaleX = 1;
            _scaleY = 1;
        }
    }
}
