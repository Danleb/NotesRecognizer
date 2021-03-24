using VoiceChanger.FormatParser;

namespace VoiceChanger.SpectrumCreator
{
    public class FastFourierTransformGPU : SpectrumCreatorGPU
    {
        public FastFourierTransformGPU(AudioContainer audioContainer) : base(audioContainer)
        {

        }

        protected override byte[] KernelSource => throw new System.NotImplementedException();
    }
}
