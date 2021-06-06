using OxyPlot;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using VoiceChanger.NoteRecognizer;
using VoiceChanger.Scalogram;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class WaveletTransformNumericalViewModel : BindableBase
    {
        public WaveletTransformNumericalViewModel() { }

        public WaveletTransformNumericalViewModel(SoundDataModel soundDataModel, ScalogramModel scalogramModel)
        {
            SoundDataModel = soundDataModel;
            ScalogramModel = scalogramModel;
            GenerateAnalysis = new DelegateCommand(() =>
            {
                var fft = new FastFourierTransformCPU(SoundDataModel.AudioContainer.Samples);
                var scalogramCreator = new SingleWaveletScalogramCreator(SoundDataModel.AudioContainer, fft);
                var frequencies = new List<FrequencyData>
                {
                    new FrequencyData(FrequencyToAnalyze)
                };
                var settings = new GuitarWaveletSettings
                {
                    Duration = Duration
                };
                //var settings = new MorletWaveletSettings
                //{
                //    CyclesCount = CyclesCount
                //};
                var scalogramContainer = scalogramCreator.CreateScalogram(frequencies, settings, false);

                var transformedSignalFft = new FastFourierTransformCPU(scalogramContainer.Scalograms.Single().Values);
                var transformedSignalScalogramCreator = new SingleWaveletScalogramCreator(SoundDataModel.AudioContainer, transformedSignalFft);
                var settings2 = new MorletWaveletSettings
                {
                    CyclesCount = CyclesCount
                    //CyclesCount = 0.1f
                };
                var transformedSignalScalogramContainer = transformedSignalScalogramCreator.CreateScalogram(frequencies, settings2, false);

                //var transformedSignal3 = new FastFourierTransformCPU(transformedSignalScalogramContainer.Scalograms.Single().Values);
                //var scalogramCreator3 = new SingleWaveletScalogramCreator(SoundDataModel.AudioContainer, transformedSignal3);
                //var settings3 = new PeaksWaveletSettings
                //{
                //    Coef = CyclesCount
                //};
                //var container3 = scalogramCreator3.CreateScalogram(frequencies, settings3, false);

                //ShowPlot(container3);
                ShowPlot(transformedSignalScalogramContainer);
                //ShowPlot(scalogramContainer);
            });

            AnalyzeNextFrequency = new DelegateCommand(() =>
            {
                FrequencyToAnalyze++;
                GenerateAnalysis.Execute();
            });

            AnalyzePreviousFrequency = new DelegateCommand(() =>
            {
                FrequencyToAnalyze--;
                GenerateAnalysis.Execute();
            });

            FrequencyToAnalyze = 82.41f;
            CyclesCount = 0.1f;
            Duration = 0.05f;
        }

        private void ShowPlot(ScalogramContainer scalogramContainer)
        {
            var list = new List<DataPoint>();
            var n = 1;
            for (int i = 0; i < scalogramContainer.Scalograms.Single().Values.Length; i++)
            {
                float v = scalogramContainer.Scalograms.Single().Values[i];
                list.Add(new DataPoint(n++, v));
            }
            Points = list;
        }

        #region Commands

        public DelegateCommand GenerateAnalysis { get; }
        public DelegateCommand AnalyzeNextFrequency { get; }
        public DelegateCommand AnalyzePreviousFrequency { get; }

        #endregion

        #region Properties

        public ScalogramModel ScalogramModel { get; }

        private string _result;
        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        private float _frequencyToAnalyze;
        public float FrequencyToAnalyze
        {
            get { return _frequencyToAnalyze; }
            set { SetProperty(ref _frequencyToAnalyze, value); }
        }

        private SoundDataModel _soundDataModel;
        public SoundDataModel SoundDataModel
        {
            get { return _soundDataModel; }
            set { SetProperty(ref _soundDataModel, value); }
        }

        private IList<DataPoint> _points;
        public IList<DataPoint> Points
        {
            get { return _points; }
            set { SetProperty(ref _points, value); }
        }

        private float _cyclesCount;
        public float CyclesCount
        {
            get { return _cyclesCount; }
            set { SetProperty(ref _cyclesCount, value); }
        }

        private WaveletType _waveletType;
        public WaveletType WaveletType
        {
            get { return _waveletType; }
            set { SetProperty(ref _waveletType, value); }
        }

        private float _duration;
        public float Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        #endregion
    }
}
