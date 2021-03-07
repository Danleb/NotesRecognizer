using System;
using System.IO;

namespace VoiceChanger.Utils
{
    public static class Samples
    {
        public static string GetSamplePath(string fileName)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var folder = Path.Combine(currentDirectory.GetParentFolder(4), "Samples");
            var path = Path.Combine(folder, fileName);
            return path;
        }

        public static string Wave100hz => GetSamplePath("100Hz_44100Hz_16bit_05sec.wav");
        public static string Wave250hz => GetSamplePath("250Hz_44100Hz_16bit_05sec.wav");
        public static string Wave440hz => GetSamplePath("440Hz_44100Hz_16bit_05sec.wav");
    }
}
