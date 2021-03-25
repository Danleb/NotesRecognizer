using System.Reactive.Subjects;

namespace VoiceChangerApp.Utils
{
    public static class SubjectUtils
    {
        public static void Invoke(this Subject<bool> subject)
        {
            subject.OnNext(false);
        }
    }
}
