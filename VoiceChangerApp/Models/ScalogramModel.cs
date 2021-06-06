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

        public ScalogramModel(ILogger<ScalogramModel> logger, SoundDataModel soundDataModel, IErrorModel errorModel)
        {
            _logger = logger;
            _soundDataModel = soundDataModel;
            _errorModel = errorModel;
            CreateScalogram.SubscribeAsync(CreateScalogramImpl);
        }

        #region Commands

        public Subject<ScalogramCreationSettings> CreateScalogram { get; } = new();

        #endregion

        #region Events

        public Subject<ScalogramContainer> OnScalogramCreated { get; } = new();

        public BehaviorSubject<CalculationState> OnScalogramCalculationState { get; } = new(CalculationState.None);

        #endregion

        #region Data

        public ScalogramContainer ScalogramContainer { get; private set; }

        public ScalogramCreator ScalogramCreator { get; private set; }

        #endregion

        private void CreateScalogramImpl(ScalogramCreationSettings settings)
        {
            switch (settings)
            {
                case LinearScalogramCreationSettings linearSettings: CreateScalogramLinearImpl(linearSettings); break;
                case GuitarScalogramCreationSettings guitarSettings: CreateScalogramGuitarImpl(guitarSettings); break;
                case HarmonicsScalogramCreationSettings harmonicsSettings: CreateScalogramHarmonics(harmonicsSettings); break;
                case GuitarHitsScalogramCreationSettings guitarHitsSettings: CreateScalogramGuitarHitsImpl(guitarHitsSettings); break;
                default: throw new NotImplementedException($"Not recognized scalogram creation type: {settings.GetType()}");
            }
        }

        private void CreateScalogramLinearImpl(LinearScalogramCreationSettings settings)
        {
            try
            {
                var frequencies = new List<FrequencyData>();
                for (var frequency = settings.FrequencyFrom; frequency <= settings.FrequencyTo; frequency += settings.FrequencyStep)
                {
                    frequencies.Add(new(frequency));
                }

                CreateScalogramByFrequencies(frequencies, settings);
            }
            catch (Exception e)
            {
                _errorModel.RaiseError(e);
                ScalogramContainer = null;
                OnScalogramCalculationState.OnNext(CalculationState.ErrorHappened);
            }
        }

        private void CreateScalogramGuitarImpl(GuitarScalogramCreationSettings settings)
        {
            try
            {
                var frequencies = GuitarTuningNotesCreator.GetStringsFrequencies(settings.TonesCount);
                CreateScalogramByFrequencies(frequencies, settings);
            }
            catch (Exception e)
            {
                _errorModel.RaiseError(e);
                ScalogramContainer = null;
                OnScalogramCalculationState.OnNext(CalculationState.ErrorHappened);
            }
        }

        private void CreateScalogramHarmonics(HarmonicsScalogramCreationSettings settings)
        {
            try
            {
                var frequencies = GuitarTuningNotesCreator.GetStringHarmonics(settings.StringNumber, settings.ToneIndex, settings.HarmomicsCount);
                CreateScalogramByFrequencies(frequencies, settings);
            }
            catch (Exception e)
            {
                _errorModel.RaiseError(e);
                ScalogramContainer = null;
                OnScalogramCalculationState.OnNext(CalculationState.ErrorHappened);
            }
        }

        private void CreateScalogramByFrequencies(List<FrequencyData> frequencies, ScalogramCreationSettings settings)
        {
            try
            {
                if (_soundDataModel.AudioContainer == null)
                {
                    _logger.LogWarning("Current AudioContainer is null.");
                    return;
                }

                _logger.LogInformation("Started creating scalogram.");
                OnScalogramCalculationState.OnNext(CalculationState.Calculating);

                //if (ScalogramCreator?.AudioContainer != _soundDataModel.AudioContainer)
                //{
                //todo change data passing
                var ft = new FastFourierTransformCPU(_soundDataModel.AudioContainer.Samples);
                var scalogramCreator = new SingleWaveletScalogramCreator(_soundDataModel.AudioContainer, ft);
                ScalogramCreator = scalogramCreator;
                _logger.LogInformation("Creating new ScalogramCreator");
                //}
                //else
                //{
                //    _logger.LogInformation("Using cached ScalogramCreator");
                //}

                frequencies.Sort();

                _logger.LogInformation($"Frequencies count: {frequencies.Count}");
                if (frequencies.Count < 10)
                {
                    frequencies.ForEach(v => _logger.LogInformation(v.ToString()));
                }

                ScalogramContainer = scalogramCreator.CreateScalogram(frequencies, settings.WaveletTransformSettings);


                //
                //var scalogramContainer = ScalogramCreator.CreateScalogram(frequencies, settings.WaveletTransformSettings);
                //var transformedSignalFft = new FastFourierTransformCPU(scalogramContainer.Scalograms.Single().Values);
                //var transformedSignalScalogramCreator = new ScalogramCreator(_soundDataModel.AudioContainer, transformedSignalFft);
                //var settings2 = new MorletWaveletSettings
                //{
                //    CyclesCount = 0.1f
                //};
                //var transformedSignalScalogramContainer = transformedSignalScalogramCreator.CreateScalogram(frequencies, settings2);
                //ScalogramContainer = transformedSignalScalogramContainer;
                //



                //ScalogramContainer = new ScalogramContainer(10, 2);                
                //var range = Enumerable.Range(1, 10).Select(v => v / 10.0f).ToArray();
                //ScalogramContainer.SetFrequencyData(0, range);
                //ScalogramContainer.SetFrequencyData(1, range.Reverse().ToArray());

                _logger.LogInformation("Finished calculating scalogram.");
                OnScalogramCreated.OnNext(ScalogramContainer);
                OnScalogramCalculationState.OnNext(CalculationState.Finished);
            }
            catch (Exception e)
            {
                HandleScalogramCreationError(e);
            }
        }

        private void HandleScalogramCreationError(Exception e)
        {
            _errorModel.RaiseError(e);
            ScalogramContainer = null;
            OnScalogramCalculationState.OnNext(CalculationState.ErrorHappened);
        }

        private void CreateScalogramGuitarHitsImpl(GuitarHitsScalogramCreationSettings guitarHitsSettings)
        {
            try
            {
                if (_soundDataModel.AudioContainer == null)
                {
                    _logger.LogWarning("Current AudioContainer is null.");
                    return;
                }

                _logger.LogInformation("Started creating scalogram.");
                OnScalogramCalculationState.OnNext(CalculationState.Calculating);


                var guitarHitsScalogramCreator = new GuitarHitsScalogramCreator(_soundDataModel.AudioContainer);
                ScalogramCreator = guitarHitsScalogramCreator;

                var frequenciesToAnalyze = GuitarTuningNotesCreator.GetStringsFrequencies(5);
                ScalogramContainer = guitarHitsScalogramCreator.CreateScalogram(frequenciesToAnalyze, guitarHitsSettings.GuitarHitsScalogramSettings, true);

                _logger.LogInformation("Finished calculating scalogram.");
                OnScalogramCreated.OnNext(ScalogramContainer);
                OnScalogramCalculationState.OnNext(CalculationState.Finished);
            }
            catch (Exception e)
            {
                HandleScalogramCreationError(e);
            }
        }
    }
}
