using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using VoiceChanger.NoteRecognizer;
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.ViewModels
{
    public class NotesRecognizingViewModel : BindableBase
    {
        public NotesRecognizingViewModel() { }

        public NotesRecognizingViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
            Recognize = new DelegateCommand(RecognizeImpl);
        }

        public SoundDataModel SoundDataModel { get; }

        #region Properties

        private NoteContainer _noteContainer;
        public NoteContainer NoteContainer
        {
            get { return _noteContainer; }
            set { SetProperty(ref _noteContainer, value); }
        }

        #endregion

        #region Commands

        public DelegateCommand Recognize { get; }

        #endregion

        private void RecognizeImpl()
        {

        }
    }
}
