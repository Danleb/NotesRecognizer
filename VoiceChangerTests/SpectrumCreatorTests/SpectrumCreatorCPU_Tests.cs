using NUnit.Framework;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChanger.Utils;

namespace VoiceChangerTests.SpectrumCreatorTests
{
    [TestFixture]
    public class SpectrumCreatorCPU_Tests
    {
        [Test]
        public void Sinus100Hz_Spectrum()
        {
            var audioContainer = AudioLoader.Load(Samples.Wave100hz);
            var spectrumCreator = new SpectrumCreatorCPU(audioContainer);
            var settings = new SpectrumCreatorSettings(audioContainer)
            {
                SpectrumMinFrequency = 20,
                SpectrumMaxFrequency = 20_000,
                SpectrumFrequencyStep = 100
            };
            var spectrumContainer = spectrumCreator.CreateSpectrum(settings);
            var amplitude100 = spectrumContainer.GetSliceForSignal(200).GetAmplitudeForFrequency(100);
            var amplitude50 = spectrumContainer.GetSliceForSignal(200).GetAmplitudeForFrequency(50);
            var amplitude150 = spectrumContainer.GetSliceForSignal(200).GetAmplitudeForFrequency(150);
            Assert.True(amplitude100 > 0.9f);
            Assert.True(amplitude50 < 0.1f);
            Assert.True(amplitude150 > 0.1f);
        }
    }
}
