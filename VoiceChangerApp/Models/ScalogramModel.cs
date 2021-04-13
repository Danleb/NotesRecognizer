using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using VoiceChanger.Scalogram;
using VoiceChanger.SpectrumCreator;

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
                _errorModel.RaiseError(e);
            }
        }
    }
}
