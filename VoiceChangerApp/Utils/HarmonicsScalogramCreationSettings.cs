namespace VoiceChangerApp.Utils
{
    public class HarmonicsScalogramCreationSettings : ScalogramCreationSettings
    {
        public float BaseFrequency { get; set; }
        public int StringNumber { get; set; }
        public int ToneIndex { get; set; }
        public bool ByStringNumber { get; set; }
        public bool ByBaseFrequency => !ByStringNumber;
        public int HarmomicsCount { get; set; }
    }
}
