using OxyPlot;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoiceChanger.SpectrumCreator;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class WaveletTransformNumericalViewModel : BindableBase
    {
        public WaveletTransformNumericalViewModel() { }

        public WaveletTransformNumericalViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;

            GenerateAnalysis = new DelegateCommand(() =>
            {
                SoundDataModel.AudioContainer.Normalize();
                var cwt = new WaveletTransformCPU(SoundDataModel.AudioContainer);
                var fs = cwt.CreateScalogram(FrequencyToAnalyze);
                //var sb = new StringBuilder();
                var list = new List<DataPoint>();
                var n = 1;
                foreach (var v in fs)
                {
                    list.Add(new DataPoint(n++, v));

                    //sb.Append(v);
                    //sb.Append(Environment.NewLine);
                }
                Points = list;

                //var path = "Result.txt";

                //if (File.Exists(path))
                //{
                //    File.Delete(path);
                //}

                //var sw = new StreamWriter(path);
                //sw.Write(sb.ToString());
                //sw.Close();
                //Result = sb.ToString();
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
        }

        #region Commands

        public DelegateCommand GenerateAnalysis { get; }
        public DelegateCommand AnalyzeNextFrequency { get; }
        public DelegateCommand AnalyzePreviousFrequency { get; }

        #endregion

        #region Properties

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

        #endregion
    }
}
