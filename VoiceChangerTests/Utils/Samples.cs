using System;
using System.IO;
using VoiceChanger.Utils;

namespace VoiceChangerTests.Utils
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

        public static string Wave100hz => Samples.GetSamplePath("100Hz_44100Hz_16bit_05sec.wav");
        public static string Wave250hz => Samples.GetSamplePath("250Hz_44100Hz_16bit_05sec.wav");
        public static string Wave440hz => Samples.GetSamplePath("440Hz_44100Hz_16bit_05sec.wav");
    }
}
