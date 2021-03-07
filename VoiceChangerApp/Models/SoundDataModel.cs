using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;

namespace VoiceChangerApp.Models
{
    public class SoundDataModel : BindableBase
    {
        public SoundDataModel()
        {
            LoadDefaultFile();
        }

        public Subject<bool> SoundTrackLoaded = new();
        public Subject<Exception> OnException = new();
        public Subject<bool> IsLoading = new();

        public AudioContainer AudioContainer { get; private set; }
        public SpectrumCreator SpectrumCreator { get; private set; }
        public SpectrumContainer SpectrumContainer => SpectrumCreator.SpectrumContainer;
        public bool IsAudioContainerCreated { get; set; }

        public void LoadFile(string path)
        {
            Task.Run(() =>
            {
                try
                {
                    IsLoading.OnNext(true);
                    AudioContainer = AudioLoader.Load(path);
                    SoundTrackLoaded.OnNext(true);
                }
                catch (Exception e)
                {
                    SoundTrackLoaded.OnNext(false);
                    OnException.OnNext(e);
                }
                finally
                {
                    IsLoading.OnNext(false);
                }
            });
        }

        [Conditional("DEBUG")]
        private void LoadDefaultFile()
        {
            LoadFile(Samples.Wave100hz);
        }
    }
}
