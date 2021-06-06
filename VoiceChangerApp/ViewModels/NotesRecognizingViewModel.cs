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

        public NotesRecognizingViewModel(SoundDataModel soundDataModel, NotesRecognizingModel notesRecognizingModel)
        {
            SoundDataModel = soundDataModel;
            NotesRecognizingModel = notesRecognizingModel;

            Recognize = new DelegateCommand(() =>
            {
                notesRecognizingModel.CreateNoteContainer.OnNext(SoundDataModel.AudioContainer);
            });
            NotesRecognizingModel.OnNoteContainer
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnNoteContainerCreated);
            NotesRecognizingModel
                .OnNoteContainerCalculationState
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnNoteContainerCalculationState);
        }

        public SoundDataModel SoundDataModel { get; }
        public NotesRecognizingModel NotesRecognizingModel { get; }

        #region Properties

        private NoteContainer _noteContainer;
        public NoteContainer NoteContainer
        {
            get { return _noteContainer; }
            set { SetProperty(ref _noteContainer, value); }
        }

        private string _tablature;
        public string Tablature
        {
            get { return _tablature; }
            set { SetProperty(ref _tablature, value); }
        }

        #endregion

        #region Commands

        public DelegateCommand Recognize { get; }

        #endregion

        private void OnNoteContainerCreated(NoteContainer noteContainer)
        {
            Tablature = new NotesToTablatureConverter(noteContainer).CreateTablature(-1);
        }

        private void OnNoteContainerCalculationState(CalculationState state)
        {

        }
    }
}
