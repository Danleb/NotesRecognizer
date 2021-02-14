using NUnit.Framework;
using System;
using System.IO;
using VoiceChanger.FormatParser;
using VoiceChanger.Utils;

namespace VoiceChangerTests.FormatParserTests
{
    class WAV_Loader_Tests
    {
        string Sample1Path => GetSamplePath("Sample1.wav");
        string Sample2Path => GetSamplePath("Sample2.wav");
        string Sample3Path => GetSamplePath("Sample3.wav");
        string Sample4Path => GetSamplePath("100Hz_44100Hz_16bit_05sec.wav");
        string Sample5Path => GetSamplePath("250Hz_44100Hz_16bit_05sec.wav");
        string Sample6Path => GetSamplePath("440Hz_44100Hz_16bit_05sec.wav");

        private string GetSamplePath(string fileName)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var folder = Path.Combine(currentDirectory.GetParentFolder(4), "Samples");
            var path = Path.Combine(folder, fileName);
            return path;
        }

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