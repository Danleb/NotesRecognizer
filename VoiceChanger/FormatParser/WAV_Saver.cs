using System.IO;
using System.Text;

namespace VoiceChanger.FormatParser
{
    public class WAV_Saver
    {
        public WAV_Saver(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer;
        }

        public AudioContainer AudioContainer { get; }

        public void Save(string path)
        {
            BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.CreateNew));

            //Encoding.ASCII.GetBytes("WAVE");



            bw.Close();
        }
    }
}
