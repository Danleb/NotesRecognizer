namespace VoiceChangerApp.Views.SoundViews
{
    public class OrthographicProjection
    {
        public OrthographicProjection()
        {
            Bottom = -1;
            Top = 1;
            Left = -1;
            Right = 1;
            Far = -1;
            Near = 1;
        }

        public double[] Matrix = new double[16];
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float Far { get; set; }
        public float Near { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

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
    }
}
