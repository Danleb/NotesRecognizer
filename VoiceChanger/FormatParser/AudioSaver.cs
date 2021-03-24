using System;

namespace VoiceChanger.FormatParser
{
    public static class AudioSaver
    {
        public static void SaveFile(AudioContainer audioContainer, FormatType formatType, string path)
        {
            switch (formatType)
            {
                case FormatType.WAV: new WAV_Saver(audioContainer).Save(path); break;
                default:
                    throw new Exception($"Not supported audio format: {formatType}.");
            }
        }
    }
}
