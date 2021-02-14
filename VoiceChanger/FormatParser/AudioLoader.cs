using System;
using System.IO;

namespace VoiceChanger.FormatParser
{
    public static class AudioLoader
    {
        public static AudioContainer Load(string path)
        {
            string extension = Path.GetExtension(path).ToLower();

            switch (extension)
            {
                case ".wav":
                    return new WAV_Loader(path).CreateContainer();
                default:
                    throw new Exception($"Unrecognized audio format: {extension}.");
            }
        }
    }
}
