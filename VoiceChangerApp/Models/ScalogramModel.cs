using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using VoiceChanger.Scalogram;
using VoiceChanger.SpectrumCreator;

namespace VoiceChangerApp.Models
{
    public class ScalogramModel
    {
        private readonly ILogger _logger;
        private readonly SoundDataModel _soundDataModel;
        private readonly ErrorModel _errorModel;
        private ScalogramCreator _scalogramCreator;

        public ScalogramModel(ILogger<ScalogramModel> logger, SoundDataModel soundDataModel, ErrorModel errorModel)
        {
            _logger = logger;
            _soundDataModel = soundDataModel;
            _errorModel = errorModel;
            CreateScalogram.Subscribe(CreateScalogramImpl);
        }

        #region Commands

        public readonly Subject<List<float>> CreateScalogram = new();

        #endregion

        #region Events

        public readonly Subject<ScalogramContainer> OnScalogramCreated = new();

        #endregion

        #region Data

        public ScalogramContainer ScalogramContainer { get; private set; }

        #endregion

        private void CreateScalogramImpl(List<float> frequencies)
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

                ScalogramContainer = _scalogramCreator.CreateScalogram(frequencies);

                _logger.LogInformation("Finished calculating scalogram.");
            }
            catch (Exception e)
            {
                ScalogramContainer = null;
                _errorModel.RaiseError(e);
            }
        }
    }
}
