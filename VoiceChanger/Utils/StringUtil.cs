namespace VoiceChanger.Utils
{
    public static class StringUtil
    {
        public static unsafe int GetCStrLength(byte* str)
        {
            int length = 0;
            while (*str != 0)
            {
                length++;
                str++;
            }
            return length;
        }
    }
}
