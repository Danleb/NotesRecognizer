using VoiceChanger.FormatParser;

namespace VoiceChangerApp.Utils
{
    public class AudioPlayer
    {
        public AudioPlayer(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer;
        }

        public AudioContainer AudioContainer { get; }
        public bool IsPlaying { get; private set; }

        public void Play()
        {

        }

        public void Play(float timeStart)
        {

        }

        public void Pause()
        {

        }

        public void Resume()
        {

        }

        public void Stop()
        {

        }
    }
}
