using System;
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

            var fileInfo = new FileInfo(Path);
            if (fileInfo.Length - WAV_Header.HeaderSize != header.FileSize)
            {
                throw new Exception("Invalid file size field value.");
            }

            if (header.SignatureWave != "WAVE")
            {
                throw new Exception("Invalid format WAVE signature.");
            }

            var dataChunkFound = false;
            var fmtChunkFound = false;
            FmtChunk fmtChunk = new();
            RIFF_Chunk dataChunk = new();

            while (!dataChunkFound)
            {
                var chunkHeader = binaryReader.ReadStruct<RIFF_Chunk>();
                var isPaddingByteNeeded = chunkHeader.ChunkSize % 2 != 0;

                switch (chunkHeader.ChunkName)
                {
                    case "fmt ":
                        {
                            fmtChunk = binaryReader.ReadStruct<FmtChunk>();
                            fmtChunkFound = true;

                            var diffLeft = chunkHeader.ChunkSize - FmtChunk.ChunkSize;
                            binaryReader.ReadBytes(diffLeft);

                            break;
                        }

                    case "data":
                        {
                            dataChunk = chunkHeader;
                            dataChunkFound = true;
                            break;
                        }

                    default:
                        {
                            var bytesToSkip = chunkHeader.ChunkSize;
                            if (isPaddingByteNeeded)
                            {
                                bytesToSkip++;
                            }

                            binaryReader.ReadBytes(bytesToSkip);
                            break;
                        }
                }

                if (isPaddingByteNeeded && !dataChunkFound)
                {
                    binaryReader.ReadByte();
                }
            }

            if (dataChunkFound && !fmtChunkFound)
            {
                throw new Exception("fmt chunk is required but not found.");
            }

            if (fmtChunk.AudioFormat != 1)
            {
                throw new Exception("Unsupported WAVE format. Only PCM supported.");
            }

            var rawData = binaryReader.ReadBytes(dataChunk.ChunkSize);
            binaryReader.Close();

            var step = fmtChunk.NumberOfChannels * fmtChunk.BytesPerSample;
            var singleChannelDataSize = dataChunk.ChunkSize / step;
            var singleChannelData = new float[singleChannelDataSize];

            var durationSeconds = 1.0f * dataChunk.ChunkSize / fmtChunk.BytesPerSample / fmtChunk.NumberOfChannels / fmtChunk.SampleRate;

            for (int byteIndex = 0, sampleIndex = 0; byteIndex < rawData.Length; byteIndex += step, sampleIndex++)
            {
                var value = fmtChunk.BytesPerSample switch
                {
                    1 => rawData[byteIndex],
                    2 => BitConverter.ToInt16(rawData, byteIndex),
                    4 => BitConverter.ToInt32(rawData, byteIndex),
                    _ => throw new NotImplementedException("Support of bytes per sample = " + fmtChunk.BytesPerSample),
                };
                singleChannelData[sampleIndex] = value;
            }

            var container = new AudioContainer(durationSeconds, fmtChunk.SampleRate, singleChannelData);
            return container;
        }
    }
}
