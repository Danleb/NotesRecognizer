using System.Collections.Generic;

namespace VoiceChanger.NoteRecognizer
{
    public class NoteContainer
    {
        public List<(float, GuitarFrequencyData)> Sounds { get; set; } = new List<(float, GuitarFrequencyData)>();

        public void AddSound(float time, GuitarFrequencyData data)
        {
            Sounds.Add((time, data));
        }
    }
}
