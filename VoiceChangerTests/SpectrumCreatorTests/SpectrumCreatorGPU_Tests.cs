using NUnit.Framework;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChangerTests.Utils;

namespace VoiceChangerTests.SpectrumCreatorTests
{
    [TestFixture]
    public class SpectrumCreatorGPU_Tests
    {
        [Test]
        public void Sinus100Hz_Spectrum()
        {
            //var audioContainer = AudioLoader.Load(Samples.Wave100hz);
            //var spectrumCreator = new SpectrumCreatorGPU(audioContainer);
            //var settings = new SpectrumCreatorSettings(audioContainer)
            //{
            //    SpectrumMinFrequency = 20,
            //    SpectrumMaxFrequency = 20_000,
            //    SpectrumFrequencyStep = 100
            //};
            //var spectrumContainer = spectrumCreator.CreateSpectrum(settings);
            //var amplitude = spectrumContainer.GetSliceForSignal(200).GetAmplitudeForFrequency(100);
            //Assert.True(amplitude > 0);
        }
    }
}
