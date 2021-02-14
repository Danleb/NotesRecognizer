using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VoiceChanger.FormatParser
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct WAV_Header
    {
        public fixed byte SignatureRiffPointer[4];
        public Int32 FileSize;
        public fixed byte SignatureWavePointer[4];
        public fixed byte FormatChunkMarkerPointer[4];
        public Int32 subchunk1Size;
        public Int16 AudioFormat;
        public Int16 NumberOfChannels;
        public Int32 SampleRate;
        public Int32 ByteRate;
        public Int16 BlockAlign;
        public Int16 BitsPerSample;
        public fixed byte DataStringPointer[4];
        public Int32 DataSectionSize;

        public string SignatureRiff
        {
            get
            {
                fixed (byte* ptr = SignatureRiffPointer)
                {
                    return Encoding.ASCII.GetString(ptr, 4);
                }
            }
        }

        public string SignatureWave
        {
            get
            {
                fixed (byte* ptr = SignatureWavePointer)
                {
                    return Encoding.ASCII.GetString(ptr, 4);
                }
            }
        }

        public string FormatChunkMarker
        {
            get
            {
                fixed (byte* ptr = FormatChunkMarkerPointer)
                {
                    return Encoding.ASCII.GetString(ptr, 4);
                }
            }
        }

        public string DataString
        {
            get
            {
                fixed (byte* ptr = DataStringPointer)
                {
                    return Encoding.ASCII.GetString(ptr, 4);
                }
            }
        }

        public int BytesPerSample => BitsPerSample / 8;
    }
}
