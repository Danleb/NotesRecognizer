using System;
using System.ComponentModel;
using System.IO;
using VoiceChanger.FormatParser;
using VoiceChanger.Utils;

namespace VoiceChanger
{
    public class WAV_Loader
    {
        public string Path { get; }

        public WAV_Loader(string path)
        {
            Path = path;
        }

        public AudioContainer CreateContainer()
        {
            var binaryReader = new BinaryReader(File.OpenRead(Path));

            var header = binaryReader.ReadStruct<WAV_Header>();

            if (header.SignatureRiff != "RIFF")
            {
                throw new Exception("Invalid format Riff signature.");
            }

            if (header.SignatureWave != "WAVE")
            {
                throw new Exception("Invalid format WAVE signature.");
            }

            if (header.FormatChunkMarker != "fmt ")
            {
                throw new Exception("Invalid fmt signature.");
            }

            if (header.AudioFormat != 1)
            {
                throw new Exception("Unsupported WAVE format. Only PCM supported.");
            }

            if (header.subchunk1Size != 16)
            {
                throw new Exception($"Wrong {nameof(header.subchunk1Size)} value. Must be 16 but is {header.subchunk1Size}.");
            }

            if (header.DataString == "LIST")
            {
                throw new NotImplementedException();
            }
            else if (header.DataString != "data")
            {
                throw new Exception("Invalid DATA string signature.");
            }

            var rawData = binaryReader.ReadBytes(header.DataSectionSize);
            binaryReader.Close();

            var step = header.NumberOfChannels * header.BytesPerSample;
            var singleChannelDataSize = header.DataSectionSize / step;
            var singleChannelData = new float[singleChannelDataSize];

            var durationSeconds = 1.0f * header.DataSectionSize / header.BytesPerSample / header.NumberOfChannels / header.SampleRate;

            for (int byteIndex = 0, sampleIndex = 0; byteIndex < rawData.Length; byteIndex += step, sampleIndex++)
            {
                int value;
                switch (header.BytesPerSample)
                {
                    case 1: value = rawData[byteIndex]; break;
                    case 2: value = BitConverter.ToInt16(rawData, byteIndex); break;
                    case 4: value = BitConverter.ToInt32(rawData, byteIndex); break;
                    default: throw new NotImplementedException("Support of bytes per sample = " + header.BytesPerSample);
                }
                singleChannelData[sampleIndex] = value;
            }

            var container = new AudioContainer(durationSeconds, header.SampleRate, singleChannelData);
            return container;
        }
    }
}
