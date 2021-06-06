namespace VoiceChanger.NoteRecognizer
{
    public class GuitarFrequencyData : FrequencyData
    {
        public GuitarFrequencyData(float frequency, int guitarStringNumber, int toneNumber)
        {
            Frequency = frequency;
            GuitarStringNumber = guitarStringNumber;
            ToneNumber = toneNumber;
        }

        public int GuitarStringNumber { get; set; }
        public int ToneNumber { get; set; }

        public static implicit operator float(GuitarFrequencyData data)
        {
            return data.Frequency;
        }
    }
}
