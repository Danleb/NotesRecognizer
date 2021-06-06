using System;

namespace VoiceChanger.NoteRecognizer
{
    public class FrequencyData : IComparable<FrequencyData>
    {
        public FrequencyData()
        {

        }

        public FrequencyData(float frequency)
        {
            Frequency = frequency;
        }

        public float Frequency { get; set; }

        public int CompareTo(FrequencyData other)
        {
            return Frequency.CompareTo(other.Frequency);
        }

        public static implicit operator float(FrequencyData data)
        {
            return data.Frequency;
        }
    }
}
