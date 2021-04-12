using Microsoft.Extensions.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Models
{
    public class SoundDataModel : BindableBase
    {
        private readonly ILogger _logger;
        private readonly ErrorModel _errorModel;

        public SoundDataModel(ILogger<SoundDataModel> logger, ErrorModel errorModel)
        {
            _logger = logger;
            _errorModel = errorModel;
            LoadFile.SubscribeAsync(LoadFileImpl);
            LoadContainer.SubscribeAsync(LoadContainerImpl);
            CalculateSampleSignalSpectrum.SubscribeAsync(_ => CalculateCommonSignalSpectrumImpl());
            GenerateSample.SubscribeAsync(GenerateSampleImp);

            LoadDefaultData();
        }

        #region Commands

        public readonly Subject<string> LoadFile = new();
        public readonly Subject<AudioContainer> LoadContainer = new();
        public readonly Subject<bool> CalculateSampleSignalSpectrum = new();
        public readonly Subject<SampleGeneratorSettings> GenerateSample = new();
        public readonly Subject<bool> SaveCurrentSampleToFile = new();

        #endregion

        #region Events

        public readonly Subject<bool> OnSampleLoaded = new();
        public readonly Subject<bool> OnCommonSignalSpectrumCalculated = new();
        public readonly BehaviorSubject<CalculationState> OnCommonSignalSpectrumCalculationState = new(CalculationState.None);
        public readonly Subject<bool> OnIsLoading = new();
        public readonly Subject<SoundSource> OnSoundSourceChanged = new();

        #endregion

        #region Data

        public AudioContainer AudioContainer { get; private set; }
        public SpectrumCreatorGPU SpectrumCreator { get; private set; }
        public SpectrumContainer SpectrumContainer => SpectrumCreator.SpectrumContainer;
        public bool IsAudioContainerCreated => AudioContainer != null;
        public bool IsLoadedFromFile { get; private set; }
        public string Path { get; private set; }
        public SpectrumSlice CommonSignalSpectrum { get; private set; }
        public SoundSource SoundSource { get; private set; }

        #endregion

        #region Debug

        [Conditional("DEBUG")]
        private void LoadDefaultData()
        {
            //LoadDefaultFile();
            GenerateDefaultSample();
        }

        private void LoadDefaultFile()
        {
            LoadFile.OnNext(Samples.Wave100hz);
        }

        private void GenerateDefaultSample()
        {
            var settings = new SampleGeneratorSettings
            {
                SampleRate = 1024,
                Datas = new List<SignalGenerationData>
                {
                    new SignalGenerationData
                    {
                        TimeStart = 0,
                        Duration = 8,
                        Frequency = 10,
                    }
                }
            };
            GenerateSampleImp(settings);
        }

        #endregion

        private void LoadFileImpl(string path)
        {
            Path = path;
            IsLoadedFromFile = true;
            OnIsLoading.OnNext(true);

            try
            {
                _logger.LogInformation("Loading from path {Path}", path);
                AudioContainer = AudioLoader.Load(path);
                AudioContainer.Normalize();

                SoundSource = SoundSource.File;
                OnSampleLoaded.OnNext(true);
                OnSoundSourceChanged.OnNext(SoundSource);
            }
            catch (Exception e)
            {
                AudioContainer = null;
                OnSampleLoaded.OnNext(false);
                _errorModel.RaiseError(e);
            }
            finally
            {
                OnIsLoading.OnNext(false);
            }
        }

        private void GenerateSampleImp(SampleGeneratorSettings settings)
        {
            try
            {
                AudioContainer = SampleGenerator.GenerateSample(settings);

                SoundSource = SoundSource.Generated;
                OnSampleLoaded.OnNext(true);
                OnSoundSourceChanged.OnNext(SoundSource);
            }
            catch (Exception e)
            {
                AudioContainer = null;
                OnSampleLoaded.OnNext(false);
                _errorModel.RaiseError(e);
            }
        }

        private void SaveSampleToFile()
        {

        }

        private void CalculateCommonSignalSpectrumImpl()
        {
            if (!IsAudioContainerCreated)
            {
                return;
            }

            try
            {
                OnCommonSignalSpectrumCalculationState.OnNext(CalculationState.Calculating);
                //var count = (int)Math.Pow(2, (int)Math.Log2(AudioContainer.SamplesCount));
                var fft = new FastFourierTransformCPU(AudioContainer.Samples).CreateTransformZeroPadded();
                //var fft = new FastFourierTransformCPU(AudioContainer.Samples).CreateTransform(0, count);                
                //CommonSignalSpectrum = fft.CreateSpectrum(0, count);
                CommonSignalSpectrum = FastFourierTransformCPU.ConvertToSpectrumSlice(fft);

                //todo put out
                var zeroPaddedDuration = (float)fft.Length / AudioContainer.SampleRate;
                foreach (var data in CommonSignalSpectrum.Datas)
                {
                    data.Frequency /= zeroPaddedDuration;
                }

                OnCommonSignalSpectrumCalculated.OnNext(true);
                OnCommonSignalSpectrumCalculationState.OnNext(CalculationState.Finished);
            }
            catch (Exception e)
            {
                OnCommonSignalSpectrumCalculated.OnNext(false);
                OnCommonSignalSpectrumCalculationState.OnNext(CalculationState.ErrorHappened);
                _errorModel.RaiseError(e);
            }
        }

        private void LoadContainerImpl(AudioContainer container)
        {
            AudioContainer = container ?? throw new ArgumentNullException(nameof(container));
            SoundSource = SoundSource.Generated;
            OnSampleLoaded.OnNext(true);
            OnSoundSourceChanged.OnNext(SoundSource);
        }
    }
}
