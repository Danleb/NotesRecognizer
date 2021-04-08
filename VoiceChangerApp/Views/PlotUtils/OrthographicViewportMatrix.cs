﻿using System.Numerics;

namespace VoiceChangerApp.Views.SoundViews
{
    public class OrthographicViewportMatrix
    {
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

        public void DoScaleX(float newScale)
        {
            var oldWidth = Width;
            var newWidth = oldWidth * newScale;
            var halfWidth = newWidth / 2.0f;
            var center = CenterX;
            Left = center - halfWidth;
            Right = center + halfWidth;
            UpdateMatrix();
        }

        public void DoScaleY(float newScale)
        {
            var oldHeight = Height;
            var newHeight = oldHeight * newScale;
            var halfHeight = newHeight / 2.0f;
            var center = CenterY;
            Bottom = center - halfHeight;
            Top = center + halfHeight;
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
