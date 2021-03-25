namespace VoiceChangerApp.Views.SoundViews
{
    public class BoundSquare
    {
        public BoundSquare(float left, float right, float bottom, float top)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public float Left { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Top { get; set; }
    }
}
