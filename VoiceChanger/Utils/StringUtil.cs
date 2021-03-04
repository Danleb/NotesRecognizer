using System;

namespace VoiceChanger.Utils
{
    public static class StringUtil
    {
        public static unsafe int GetCStrLength(byte* str)
        {
            Int32* strWchar = (Int32*)str;
            int length = 0;
            while (*strWchar != 0)
            {
                length++;
                strWchar++;
            }
            return length;
        }
    }
}
