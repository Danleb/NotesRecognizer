using VoiceChanger.FormatParser;

namespace VoiceChangerApp.Views.SoundViews
{
    public class SoundViewPosition
    {
        public static SoundViewPosition StartPosition() => new();

        private float time;
        private int signalIndex;
        private SelectedPositionAnchor anchor;//?

        public SoundViewPosition()
        {

        }

        public SoundViewPosition(AudioContainer audioContainer)
        {

        }

        public float Time
        {
            get => time;
            set
            {
                time = value;
            }
        }
        public int SignalIndex
        {
            get => signalIndex;
            set
            {
                signalIndex = value;
            }
        }
        public SelectedPositionAnchor Anchor
        {
            get => anchor;
            set
            {
                anchor = value;
            }
        }

        public void OffsetAbsoluteTime(float seconds)
        {

        }

        public void OffsetAbsoluteSignals(int signalsCount)
        {

        }

        public void OffsetRelative(float percent)
        {

        }

        public void SetTime(float seconds)
        {

        }

        public void SetSignalIndex(int signalIndex)
        {

        }
    }
}
