using VoiceChanger.NoteRecognizer;

namespace VoiceChanger.Scalogram
{
    public class FrequencyScalogram
    {
        public FrequencyScalogram(FrequencyData frequencyData, float[] scalogram)
        {
            FrequencyData = frequencyData;
            Values = scalogram;
        }

        public FrequencyData FrequencyData { get; set; }

        public float[] Values { get; set; }

        public int Length => Values.Length;

        public float this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }
    }
}
