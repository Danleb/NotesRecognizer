using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace VoiceChangerApp.Utils
{
    public static class SubjectUtils
    {
        public static void Invoke(this Subject<bool> subject)
        {
            subject.OnNext(false);
        }

        public static void SubscribeAsync<T>(this Subject<T> subject, Action<T> action)
        {
            subject.Subscribe(v => Task.Run(() => action(v)));
        }

        public static void SubscribeAsync<T>(this BehaviorSubject<T> subject, Action<T> action)
        {
            subject.Subscribe(v => Task.Run(() => action(v)));
        }
    }
}
