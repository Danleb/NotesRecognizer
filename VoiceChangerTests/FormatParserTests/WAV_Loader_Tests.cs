using NUnit.Framework;
using System.IO;
using VoiceChanger.FormatParser;
using VoiceChangerTests.Utils;

namespace VoiceChangerTests.FormatParserTests
{
    [TestFixture]
    public class WAV_Loader_Tests
    {
        string Sample1Path => Samples.GetSamplePath("Sample1.wav");
        string Sample2Path => Samples.GetSamplePath("Sample2.wav");
        string Sample3Path => Samples.GetSamplePath("Sample3.wav");
        string Sample4Path => Samples.Wave100hz;
        string Sample5Path => Samples.Wave250hz;
        string Sample6Path => Samples.Wave440hz;

        [Test]
        public void ParseSample1()
        {
            Assert.IsTrue(File.Exists(Sample1Path));
            var container = AudioLoader.Load(Sample1Path);
            Assert.NotNull(container);

        }

        [Test]
        public void ParseSample2()
        {
            Assert.IsTrue(File.Exists(Sample2Path));
            var container = AudioLoader.Load(Sample2Path);
            Assert.NotNull(container);

        }

        [Test]
        public void ParseSample3()
        {
            Assert.IsTrue(File.Exists(Sample3Path));
            var container = AudioLoader.Load(Sample3Path);
            Assert.NotNull(container);

        }

        [Test]
        public void ParseSample4()
        {
            Assert.IsTrue(File.Exists(Sample4Path));
            var container = AudioLoader.Load(Sample4Path);
            Assert.NotNull(container);

        }

        [Test]
        public void ParseSample5()
        {
            Assert.IsTrue(File.Exists(Sample5Path));
            var container = AudioLoader.Load(Sample5Path);
            Assert.NotNull(container);

        }

        [Test]
        public void ParseSample6()
        {
            Assert.IsTrue(File.Exists(Sample6Path));
            var container = AudioLoader.Load(Sample6Path);
            Assert.NotNull(container);

        }
    }
}