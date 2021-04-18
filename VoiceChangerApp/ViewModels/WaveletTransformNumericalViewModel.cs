using OxyPlot;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using VoiceChanger.Scalogram;
using VoiceChanger.SpectrumCreator;
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
                var scalogramCreator = new ScalogramCreator(SoundDataModel.AudioContainer, fft);
                var frequencies = new List<float>
                {
                    FrequencyToAnalyze
                };
                var scalogramContainer = scalogramCreator.CreateScalogram(frequencies, CyclesCount);

                var list = new List<DataPoint>();
                var n = 1;
                foreach (var v in scalogramContainer.ScalogramValues)
                {
                    list.Add(new DataPoint(n++, v));
                }
                Points = list;
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

            FrequencyToAnalyze = 1;
            CyclesCount = 3;
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

        private int _cyclesCount;
        public int CyclesCount
        {
            get { return _cyclesCount; }
            set { SetProperty(ref _cyclesCount, value); }
        }

        #endregion
    }
}
