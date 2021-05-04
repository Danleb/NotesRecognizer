using System.Collections.Generic;

namespace VoiceChanger.NoteRecognizer
{
    public class NoteContainer
    {
        public Dictionary<FrequencyData, List<float>> Tablature { get; set; }

    }
}
