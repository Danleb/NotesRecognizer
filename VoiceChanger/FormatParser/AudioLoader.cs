using System;
using System.Collections.Immutable;
using System.IO;

namespace VoiceChanger.FormatParser
{
    public static class AudioLoader
    {
        public readonly static ImmutableArray<string> SupportedExtensions = ImmutableArray.Create(
            ".wav"
            );

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
