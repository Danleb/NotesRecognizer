using System;
using System.Collections.Generic;
using System.Linq;

namespace VoiceChanger.NoteRecognizer
{
    public class GuitarTuningNotesCreator
    {
        public const float BaseFrequency = 440;

        public static readonly IReadOnlyDictionary<int, int> OpenStringsBaseTones = new Dictionary<int, int>
        {
            {1, -5 },
            {2, -10 },
            {3, -14 },
            {4, -19 },
            {5, -24 },
            {6, -29 },
        };

        public static float GetNoteFrequency(float baseFrequency, int tone)
        {
            if (tone == 0)
            {
                return baseFrequency;
            }

            var power = tone / 12.0;
            var frequency = baseFrequency * Math.Pow(2, power);
            return (float)frequency;
        }

        public static FrequencyData GetStringFrequency(int stringNumber, int toneShift)
        {
            var tone = OpenStringsBaseTones[stringNumber] + toneShift;
            var frequency = GetNoteFrequency(BaseFrequency, tone);
            var data = new GuitarFrequencyData(frequency, stringNumber, toneShift);
            return data;
        }

        public static List<float> GetStringFrequenciesRange(int stringNumber, int tonesCount)
        {
            var list = new List<float>();

            for (int i = 0; i < tonesCount; i++)
            {
                var frequency = GetStringFrequency(stringNumber, i);
                list.Add(frequency);
            }

            return list;
        }

        public static List<FrequencyData> GetStringsFrequencies(int tonesCount, List<int> stringNumbers = null)
        {
            if (stringNumbers == null)
            {
                stringNumbers = OpenStringsBaseTones.Keys.ToList();
            }

            var set = new SortedSet<FrequencyData>();

            foreach (var stringNumber in stringNumbers)
            {
                for (var i = 0; i < tonesCount; i++)
                {
                    var frequency = GetStringFrequency(stringNumber, i);
                    set.Add(frequency);
                }
            }
            var list = set.ToList();
            return list;
        }

        public static List<GuitarFrequencyData> GetStringsFrequencyDatas(int tonesCount, List<int> stringNumbers = null)
        {
            var datas = new List<GuitarFrequencyData>();
            foreach (var stringNumber in stringNumbers)
            {
                for (var toneIndex = 0; toneIndex < tonesCount; toneIndex++)
                {
                    var frequency = GetStringFrequency(stringNumber, toneIndex);
                    var data = new GuitarFrequencyData(frequency, stringNumber, toneIndex);
                    datas.Add(data);
                }
            }
            var list = datas.ToList();
            return list;
        }

        public static List<FrequencyData> GetStringHarmonics(int stringNumber, int toneIndex, int harmonicsCount)
        {
            var frequencies = new List<FrequencyData>();
            var baseFrequency = GetStringFrequency(stringNumber, toneIndex);
            for (int i = 0; i < harmonicsCount; i++)
            {
                var frequency = baseFrequency * MathF.Pow(2, i);
                frequencies.Add(new(frequency));
            }
            return frequencies;
        }
    }
}
