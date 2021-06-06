using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceChanger.NoteRecognizer
{
    public class NotesToTablatureConverter
    {
        private readonly NoteContainer _noteContainer;

        public NotesToTablatureConverter(NoteContainer noteContainer)
        {
            _noteContainer = noteContainer;
        }

        public string CreateTablature(int width)
        {
            var tabulatureBuilder = new StringBuilder();
            var stringsLists = new Dictionary<int, StringBuilder>();

            var signs = new string[]
            {
                "e|",
                "B|",
                "G|",
                "D|",
                "A|",
                "E|"
            };
            Enumerable.Range(1, 6).ToList().ForEach(v => stringsLists.Add(v, new StringBuilder(signs[v - 1])));

            var maxTime = _noteContainer.Sounds.Max(v => v.Item1);

            var spacesInSecond = 5;
            var step = 1.0f / spacesInSecond;
            for (float time = 0; time <= maxTime + step; time += step)
            {
                var thisTimeStampHits = _noteContainer.Sounds.Where(v => Math.Abs(v.Item1 - time) < step / 2.0f);

                if (!thisTimeStampHits.Any())
                {
                    stringsLists.Values.First().Append('-', 1);
                }

                foreach (var (_, data) in thisTimeStampHits)
                {
                    var sb = stringsLists[data.GuitarStringNumber];
                    sb.Append(data.ToneNumber);
                    sb.Append('-', 1);
                }

                var longestStringLength = stringsLists.Values.Max(v => v.Length);
                foreach (var stringBuilder in stringsLists.Values)
                {
                    var diff = longestStringLength - stringBuilder.Length;
                    if (diff > 0)
                    {
                        stringBuilder.Append('-', diff);
                    }
                }
            }

            foreach (var v in stringsLists.Values)
            {
                tabulatureBuilder.Append(v);
                tabulatureBuilder.Append(Environment.NewLine);
            }
            return tabulatureBuilder.ToString();
        }
    }
}
