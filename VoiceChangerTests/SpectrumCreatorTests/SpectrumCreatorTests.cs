using NUnit.Framework;
using VoiceChanger.FormatParser;
using VoiceChanger.SpectrumCreator;
using VoiceChangerTests.Utils;

namespace VoiceChangerTests.SpectrumCreatorTests
{
    [TestFixture]
    public class SpectrumCreatorTests
    {
        [Test]
        public void SinusSpectrum()
        {
            var audioContainer = AudioLoader.Load(Samples.Wave100hz);
            var spectrumCreator = new SpectrumCreator(audioContainer);
            var spectrumContainer = spectrumCreator.CreateSpectrum();
            var frequency = -1;
            Assert.AreEqual(100, frequency);
        }
    }
}
