using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VoiceChanger.FormatParser
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct WAV_Header
    {
        public const int HeaderSize = 8;
        public const int NameSize = 4;

        public fixed byte SignatureRiffPointer[NameSize];
        public Int32 FileSize;
        public fixed byte SignatureWavePointer[NameSize];

        public string SignatureRiff
        {
            get
            {
                fixed (byte* ptr = SignatureRiffPointer)
                {
                    return Encoding.ASCII.GetString(ptr, NameSize);
                }
            }
        }

        public string SignatureWave
        {
            get
            {
                fixed (byte* ptr = SignatureWavePointer)
                {
                    return Encoding.ASCII.GetString(ptr, NameSize);
                }
            }
        }
    }
}
