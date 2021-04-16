using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VoiceChanger.FormatParser
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct RIFF_Chunk
    {
        private const int ChunkNameSize = 4;
        public fixed byte ChunkNamePointer[ChunkNameSize];
        public Int32 ChunkSize;

        public string ChunkName
        {
            get
            {
                fixed (byte* ptr = ChunkNamePointer)
                {
                    return Encoding.ASCII.GetString(ptr, ChunkNameSize);
                }
            }
        }
    }
}
