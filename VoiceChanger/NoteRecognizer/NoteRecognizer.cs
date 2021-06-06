using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VoiceChanger.FormatParser;
using VoiceChanger.Scalogram;

namespace VoiceChanger.NoteRecognizer
{
    public class NoteRecognizer
    {
        private readonly ILogger _logger;

        public NoteRecognizer(AudioContainer audioContainer, ILogger logger)
        {
            AudioContainer = audioContainer;
            _logger = logger;
        }

        public AudioContainer AudioContainer { get; }
        public ScalogramCreator ScalogramCreator { get; }

        public NoteContainer CreateNoteContainer(List<int> stringsToAnalyze, int tonesCount)
        {
            var noteContainer = new NoteContainer();

            var frequencies = GuitarTuningNotesCreator.GetStringsFrequencies(tonesCount, stringsToAnalyze);
            _logger.LogTrace($"Frequencies count = {frequencies.Count}");
            //var frequencies = GuitarTuningNotesCreator.GetStringsFrequencyDatas(tonesCount, stringsToAnalyze);

            var threshold = 0.05f;//0.1 //1.5e10;
            var settings = new GuitarHitsScalogramSettings
            {
                Duration = 0.05f,
                CyclesCount = 0.2f
            };
            //var datas = frequencies.Select(v => new FrequencyData( FrequencyData)v).ToList();
            var scalogramsContainer = new GuitarHitsScalogramCreator(AudioContainer).CreateScalogram(frequencies, settings, true);
            //var scalogramsContainer = new GuitarHitsScalogramCreator(AudioContainer).CreateScalogram(datas, settings, true);

            foreach (var scalogram in scalogramsContainer.Scalograms)
            {
                var localMaximumIndexes = new List<int>();
                
                var oneSoundTreshold = 0.2f//0.11337f 
                    * AudioContainer.SampleRate;
                var wasBigger = false;

                for (int i = 1; i < scalogram.Length - 1; i++)
                {
                    var bigger = scalogram[i] > scalogram[i - 1];

                    if (wasBigger && !bigger)
                    {
                        if (i - localMaximumIndexes.LastOrDefault() > oneSoundTreshold)
                        {
                            localMaximumIndexes.Add(i);
                        }
                    }

                    wasBigger = bigger;
                }

                //var vals = localMaximumIndexes.Select(v => scalogram[v]).ToList();
                //var max = scalogram.Values.Max();
                //var maxInd = scalogram.Values.ToList().IndexOf(max);//.((v, ind) => v == max).Select(;

                localMaximumIndexes = localMaximumIndexes.Where(v => scalogram[v] >= threshold).ToList();
                //_logger.LogInformation($"For frequency local maximums count ${localMaximumIndexes.Length}");

                //var maxs = scalogram.Values.Where(v => v >= threshold).ToList();
                _logger.LogTrace($"Frequency: {scalogram.FrequencyData.Frequency}");

                if (Math.Abs(scalogram.FrequencyData.Frequency - 103.83) < 2)
                {
                    int q = 5;
                }

                for (int localMaximumCounter = 0; localMaximumCounter < localMaximumIndexes.Count; localMaximumCounter++)
                {
                    int localMaximumIndex = localMaximumIndexes[localMaximumCounter];
                    var passes = scalogramsContainer.Plots[scalogram.FrequencyData][localMaximumIndex] > 2e+8;//1e+8;//5e+7;

                    if (passes)
                    {
                        _logger.LogTrace("Passes");
                        var timeRatio = (float)localMaximumIndex / scalogram.Length;
                        var time = AudioContainer.Duration * timeRatio;
                        noteContainer.Sounds.Add((time, (GuitarFrequencyData)scalogram.FrequencyData));
                    }
                }
            }

            //
            const float TimeEpsilon = 0.4f;
            const float HarmonicsEpsilon = 0.03f;
            const int BaseToneHarmonics = 1;

            var filteredNoteContainer = new NoteContainer();

            foreach (var (currentTime, currentData) in noteContainer.Sounds)
            {
                var baseSounds = noteContainer.Sounds.Where(pair =>
                {
                    var time = pair.Item1;
                    var data = pair.Item2;

                    var isSameTime = Math.Abs(currentTime - time) < TimeEpsilon;

                    var harmonicsNumber = currentData.Frequency / data.Frequency;
                    var integerHarnomics = Math.Round(harmonicsNumber);
                    var harmonicsDiff = Math.Abs(harmonicsNumber - integerHarnomics);
                    var isHarmonicsOfFrequency = harmonicsDiff < HarmonicsEpsilon;

                    if (isSameTime && isHarmonicsOfFrequency && integerHarnomics != BaseToneHarmonics)
                    {
                        return true;
                    }

                    return false;
                });

                if (!baseSounds.Any())
                {
                    filteredNoteContainer.AddSound(currentTime, currentData);
                }
            }

            //
            var bestTablatureFinder = new BestTablatureFinder(noteContainer);
            var optimalTablature = bestTablatureFinder.CreateOptimalTablatures();

            return filteredNoteContainer;
        }
    }
}
