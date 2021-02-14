using System.IO;

namespace VoiceChanger.Utils
{
    public static class PathUtils
    {
        public static string GetParentFolder(this string path, int level)
        {
            for (int i = 0; i < level; i++)
            {
                path = Path.GetDirectoryName(path);
            }
            return path;
        }
    }
}
