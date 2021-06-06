using System.Collections.Generic;
using System.Linq;

namespace VoiceChanger.NoteRecognizer
{
    public class BestTablatureFinder
    {
        private readonly NoteContainer _noteContainer;

        public BestTablatureFinder(NoteContainer noteContainer)
        {
            _noteContainer = noteContainer;
        }

        public List<NoteContainer> CreateOptimalTablatures()
        {
            var list = new List<NoteContainer>();



            return list;
        }


        public float Estimate(NoteContainer noteContainer)
        {
            var maxTone = noteContainer.Sounds.Max(v => v.Item2.ToneNumber);
            var stringsCount = 6;
            var tonesCount = maxTone + 1;
            var box = new int[stringsCount, tonesCount];
            foreach (var (_, data) in noteContainer.Sounds)
            {
                box[data.GuitarStringNumber - 1, data.ToneNumber]++;
            }

            var value = 0.0f;

            for (int i = 0; i < stringsCount; i++)
            {
                //for ()
            }

            return 0;
        }
    }
}
