using System;
using System.Runtime.InteropServices;

namespace VoiceChanger.FormatParser
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct FmtChunk
    {
        public const int ChunkSize = 16;

        public Int16 AudioFormat;
        public Int16 NumberOfChannels;
        public Int32 SampleRate;
        public Int32 ByteRate;
        public Int16 BlockAlign;
        public Int16 BitsPerSample;

        public int BytesPerSample => BitsPerSample / 8;
    }
}
