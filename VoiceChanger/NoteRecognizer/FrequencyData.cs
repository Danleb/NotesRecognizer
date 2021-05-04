namespace VoiceChanger.NoteRecognizer
{
    public class FrequencyData
    {
        public FrequencyData(float frequency, int guitarStringNumber, int toneNumber)
        {
            Frequency = frequency;
            GuitarStringNumber = guitarStringNumber;
            ToneNumber = toneNumber;
        }

        public float Frequency { get; set; }
        public int GuitarStringNumber { get; set; }
        public int ToneNumber { get; set; }

        public static implicit operator float(FrequencyData data)
        {
            return data.Frequency;
        }
    }
}
