using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VoiceChanger.Utils
{
    public static class BinaryReaderUtils
    {
        public unsafe static T ReadStruct<T>(this BinaryReader binaryReader) where T : unmanaged
        {
            var bytes = binaryReader.ReadBytes(sizeof(T));

            //fixed (byte* ptr = &bytes[0])
            //{
            //    return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
            //}

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
