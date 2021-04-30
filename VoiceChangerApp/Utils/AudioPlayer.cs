using System.Runtime.InteropServices;
using VoiceChanger.FormatParser;

namespace VoiceChangerApp.Utils
{
    public class AudioPlayer
    {
        private const string ApiDllName = "Suprecessor";

        public AudioPlayer(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer;
        }


        [DllImport(ApiDllName)]
        private static extern void CreateAudioPlayer();

        public AudioContainer AudioContainer { get; }
        public bool IsPlaying { get; private set; }
        public float CurrentTime { get; private set; }
        public float CurrentRatio { get; private set; }

        public void PlayFromStart()
        {

        }

        public void Play(float timeStart)
        {

        }

        public void Continue()
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
