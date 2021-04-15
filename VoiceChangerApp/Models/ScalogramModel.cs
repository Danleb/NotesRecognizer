using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using VoiceChanger.NoteRecognizer;
using VoiceChanger.Scalogram;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Models
{
    public class ScalogramModel
    {
        private readonly ILogger _logger;
        private readonly SoundDataModel _soundDataModel;
        private readonly IErrorModel _errorModel;
        private ScalogramCreator _scalogramCreator;

        public ScalogramModel(ILogger<ScalogramModel> logger, SoundDataModel soundDataModel, IErrorModel errorModel)
        {
            _logger = logger;
            _soundDataModel = soundDataModel;
            _errorModel = errorModel;
            CreateScalogramLinear.Subscribe(CreateScalogramLinearImpl);
            CreateScalogramGuitar.Subscribe(CreateScalogramGuitarImpl);
        }

        #region Commands

        public readonly Subject<LinearScalogramCreationSettings> CreateScalogramLinear = new();

        public readonly Subject<GuitarScalogramCreationSettings> CreateScalogramGuitar = new();

        #endregion

        #region Events

        public readonly Subject<ScalogramContainer> OnScalogramCreated = new();

        #endregion

        #region Data

        public ScalogramContainer ScalogramContainer { get; private set; }

        #endregion

        private void CreateScalogramLinearImpl(LinearScalogramCreationSettings settings)
        {
            try
            {
                var frequencies = new List<float>();
                for (var frequency = settings.FrequencyFrom; frequency <= settings.FrequencyTo; frequency += settings.FrequencyStep)
                {
                    frequencies.Add(frequency);
                }

                CreateScalogram(frequencies);
            }
            catch (Exception e)
            {
                ScalogramContainer = null;
                _errorModel.RaiseError(e);
            }
        }

        private void CreateScalogramGuitarImpl(GuitarScalogramCreationSettings settings)
        {
            try
            {
                var frequencies = GuitarTuningNotesCreator.GetStringsFrequencies(settings.TonesCount);
                CreateScalogram(frequencies);
            }
            catch (Exception e)
            {
                _errorModel.RaiseError(e);
            }
        }

        private void CreateScalogram(List<float> frequencies)
        {
            try
            {
                if (_soundDataModel.AudioContainer == null)
                {
                    _logger.LogWarning("Current AudioContainer is null.");
                    return;
                }

                _logger.LogInformation("Started creating scalogram.");

                if (_scalogramCreator?.AudioContainer != _soundDataModel.AudioContainer)
                {
                    //todo change data passing
                    var ft = new FastFourierTransformCPU(_soundDataModel.AudioContainer.Samples);
                    _scalogramCreator = new ScalogramCreator(_soundDataModel.AudioContainer, ft);
                    _logger.LogInformation("Creating new ScalogramCreator");
                }
                else
                {
                    _logger.LogInformation("Using cached ScalogramCreator");
                }

                frequencies.Sort();
                ScalogramContainer = _scalogramCreator.CreateScalogram(frequencies);

                //ScalogramContainer = new ScalogramContainer(10, 2);                
                //var range = Enumerable.Range(1, 10).Select(v => v / 10.0f).ToArray();
                //ScalogramContainer.SetFrequencyData(0, range);
                //ScalogramContainer.SetFrequencyData(1, range.Reverse().ToArray());

                _logger.LogInformation("Finished calculating scalogram.");
                OnScalogramCreated.OnNext(ScalogramContainer);
            }
            catch (Exception e)
            {
                ScalogramContainer = null;
            }
        }
    }
}
