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

        public static float GetStringFrequency(int stringNumber, int toneShift)
        {
            var tone = OpenStringsBaseTones[stringNumber] + toneShift;
            var frequency = GetNoteFrequency(BaseFrequency, tone);
            return frequency;
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

        public static List<float> GetStringsFrequencies(int tonesCount)
        {
            var set = new SortedSet<float>();
            foreach (var stringNumber in OpenStringsBaseTones.Keys)
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
    }
}
