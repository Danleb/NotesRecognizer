using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using VoiceChanger.FormatParser;
using VoiceChanger.NoteRecognizer;
using VoiceChanger.Scalogram;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;
using VoiceChangerApp.Utils;


namespace VoiceChangerApp.Models
{
    public class NotesRecognizingModel
    {
        private readonly ILogger _logger;
        private readonly SoundDataModel _soundDataModel;
        private readonly IErrorModel _errorModel;

        public NotesRecognizingModel(ILogger<ScalogramModel> logger, SoundDataModel soundDataModel, IErrorModel errorModel)
        {
            _logger = logger;
            _soundDataModel = soundDataModel;
            _errorModel = errorModel;

            CreateNoteContainer.SubscribeAsync(CreateNoteContainerImpl);
        }

        #region Commands

        public Subject<AudioContainer> CreateNoteContainer { get; } = new();

        #endregion

        #region Events

        public Subject<NoteContainer> OnNoteContainer { get; } = new();
        public BehaviorSubject<CalculationState> OnNoteContainerCalculationState { get; } = new(CalculationState.None);

        #endregion

        #region Data

        public NoteContainer NoteContainer { get; private set; }

        #endregion

        private void CreateNoteContainerImpl(AudioContainer audioContainer)
        {
            try
            {
                OnNoteContainerCalculationState.OnNext(CalculationState.Calculating);

                var notesRecognizer = new NoteRecognizer(audioContainer, _logger);
                //var stringsToAnalyze = new List<int> { 3, 4 };
                var stringsToAnalyze = new List<int> { 1, 2, 3, 4, 5, 6 };
                NoteContainer = notesRecognizer.CreateNoteContainer(stringsToAnalyze, 13);
                //NoteContainer = notesRecognizer.CreateNoteContainer(stringsToAnalyze, 7);

                OnNoteContainerCalculationState.OnNext(CalculationState.Finished);
                OnNoteContainer.OnNext(NoteContainer);
            }
            catch (Exception e)
            {
                NoteContainer = null;
                _errorModel.RaiseError(e);
                OnNoteContainerCalculationState.OnNext(CalculationState.ErrorHappened);
            }
        }
    }
}
