using NUnit.Framework;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;

namespace VoiceChangerTests.SpectrumCreatorTests
{
    [TestFixture]
    public class FastFourierTransformCPU_Tests
    {
        [Test]
        public void Sinus100Hz_Spectrum()
        {
            //var audioContainer = AudioLoader.Load(Samples.Wave100hz);
            var container = SampleGenerator.GenerateSineSignal(100, 2048, 2048);
            var fft = new FastFourierTransformCPU(container);
            var slice = fft.CreateSpectrum();
            var amplitude50 = slice.GetAmplitudeForFrequency(50);
            var amplitude100 = slice.GetAmplitudeForFrequency(100);
            var amplitude150 = slice.GetAmplitudeForFrequency(150);
            Assert.True(amplitude50 < 0.1f);
            Assert.True(amplitude100 > 0.9f);
            Assert.True(amplitude150 > 0.1f);
        }
    }
}
