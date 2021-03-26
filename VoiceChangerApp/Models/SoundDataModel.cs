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

        public SoundDataModel(ILogger<SoundDataModel> logger)
        {
            _logger = logger;
            LoadFile.Subscribe(LoadFileImp);
            CalculateSampleSignalSpectrum.Subscribe(_ => CalculateCommonSignalSpectrumImp());
            GenerateSample.Subscribe(GenerateSampleImp);

            LoadDefaultData();
        }

        #region Commands

        public Subject<string> LoadFile = new();
        public Subject<AudioContainer> LoadContainer = new();
        public Subject<bool> CalculateSampleSignalSpectrum = new();
        public Subject<SampleGeneratorSettings> GenerateSample = new();
        public Subject<bool> SaveCurrentSampleToFile = new();

        #endregion

        #region Events


        public Subject<bool> OnSampleLoaded = new();
        public Subject<bool> OnCommonSignalSpectrumCalculated = new();
        public Subject<Exception> OnException = new();
        public Subject<bool> OnIsLoading = new();
        public Subject<SoundSource> OnSoundSourceChanged = new();

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
                SampleRate = 100,
                Datas = new List<SignalGenerationData>
                {
                    new SignalGenerationData
                    {
                        TimeStart = 0,
                        Duration = 1,
                        Frequency = 10,
                    }
                }
            };
            GenerateSampleImp(settings);
        }

        #endregion

        private void LoadFileImp(string path)
        {
            Path = path;
            IsLoadedFromFile = true;
            OnIsLoading.OnNext(true);

            try
            {
                _logger.LogInformation("Loading from path {Path}", path);
                AudioContainer = AudioLoader.Load(path);

                SoundSource = SoundSource.File;
                OnSampleLoaded.OnNext(true);
                OnSoundSourceChanged.OnNext(SoundSource);
            }
            catch (Exception e)
            {
                AudioContainer = null;
                OnSampleLoaded.OnNext(false);
                OnException.OnNext(e);
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
                OnException.OnNext(e);
            }
        }

        private void SaveSampleToFile()
        {

        }

        private void CalculateCommonSignalSpectrumImp()
        {
            if (!IsAudioContainerCreated)
            {
                return;
            }

            try
            {
                var fft = new FastFourierTransformCPU(AudioContainer);
                var count = (int)Math.Pow(2, (int)Math.Log2(AudioContainer.SamplesCount));
                CommonSignalSpectrum = fft.CreateSpectrum(0, count);
                OnCommonSignalSpectrumCalculated.OnNext(true);
            }
            catch (Exception e)
            {
                OnCommonSignalSpectrumCalculated.OnNext(false);
                OnException.OnNext(e);
            }
        }
    }
}
