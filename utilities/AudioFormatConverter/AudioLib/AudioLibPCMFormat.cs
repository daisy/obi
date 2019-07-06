using System;
using System.IO;
using System.Text;

namespace AudioLib
{
    /// <summary>
    /// Class for holding information about no. of channels, sampling rate and bit depth for PCM file or stream.
    /// </summary> 
    public class AudioLibPCMFormat
    {

        public AudioLibPCMFormat()
            : this(1, 44100, 16)
        {
        }

        public AudioLibPCMFormat(ushort channels, uint samplingRate, ushort bitDepth)
        {
            NumberOfChannels = channels;
            SampleRate = samplingRate;
            BitDepth = bitDepth;
        }

        private ushort m_Channels;
        public ushort NumberOfChannels // 1 = mono, 2 = stereo
        {
            get { return m_Channels; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Minimum number of channels is 1");
                }
                m_Channels = value;
            }
        }

        private uint m_SamplingRate;
        public uint SampleRate
        {
            get { return m_SamplingRate; }
            set
            {
                if (value != 11025
                  && value != 22050
                  && value != 44100
                  && value != 88200
                  && !(value >= 8000
                          && value <= 96000
                          && value % 8000 == 0)
                  )//Todo: check this line
                {
                    throw new ArgumentOutOfRangeException("Invalid sampling rate: " + value);
                }
                m_SamplingRate = value;
            }
        }

        private ushort m_BitDepth;
        public ushort BitDepth // number of bits per sample
        {
            get { return m_BitDepth; }
            set
            {
                if (!(value >= 8 && value <= 32 && value % 8 == 0))
                {
                    throw new ArgumentOutOfRangeException("Invalid bit depth: " + value);
                }
                m_BitDepth = value;
            }
        }

        public ushort BlockAlign // Otherwise known as "frame size", in bytes. A frame is a sequence of samples, one for each channel of the digitized audio signal.
        {
            get { return (ushort)((m_BitDepth / 8) * m_Channels); }
        }

        public uint ByteRate
        {
            get { return NumberOfChannels * SampleRate * BitDepth / 8U; }
        }

        private bool m_IsCompressed = false;
        public bool IsCompressed
        {
            get { return m_IsCompressed; }
            set
            {
                m_IsCompressed = value;
            }
        }

        /// <summary>
        /// Determines if the <see cref="PCMFormatInfo"/> is compatible with a given other <see cref="PCMDataInfo"/>
        /// </summary>
        /// <param name="pcmInfo">The other PCMDataInfo</param>
        /// <returns>A <see cref="bool"/> indicating the compatebility</returns>
        public bool IsCompatibleWith(AudioLibPCMFormat pcmInfo)
        {
            if (pcmInfo == null) return false;
            if (NumberOfChannels != pcmInfo.NumberOfChannels) return false;
            if (SampleRate != pcmInfo.SampleRate) return false;
            if (BitDepth != pcmInfo.BitDepth) return false;
            return true;
        }


        public void CopyValues(AudioLibPCMFormat pcmFormat)
        {
            NumberOfChannels = pcmFormat.NumberOfChannels;
            SampleRate = pcmFormat.SampleRate;
            BitDepth = pcmFormat.BitDepth;
        }

        public long ConvertTimeToBytes(double time)
        {
            return ConvertTimeToBytes(time, SampleRate, BlockAlign);
        }

        private static long ConvertTimeToBytes(double time, uint samplingRate, ushort frameSize)
        {
            long bytes = Convert.ToInt64((time * samplingRate * frameSize) / 1000.0);
            bytes -= bytes % frameSize;
            return bytes;
        }

        public double ConvertBytesToTime(long bytes)
        {
            return ConvertBytesToTime(bytes, SampleRate, BlockAlign);
        }

        private static double ConvertBytesToTime(long bytes, uint samplingRate, ushort frameSize)
        {
            bytes -= bytes % frameSize;
            return (1000.0 * bytes) / (samplingRate * frameSize); //Convert.ToDouble
        }


        /// <summary>
        /// Compares the data in two data streams for equality
        /// </summary>
        /// <param name="s1">The first </param>
        /// <param name="s2"></param>
        /// <param name="length">The length of the data to compare</param>
        /// <returns>A <see cref="bool"/> indicating data equality</returns>
        public static bool CompareStreamData(Stream s1, Stream s2, int length)
        {
            byte[] d1 = new byte[length];
            byte[] d2 = new byte[length];
            if (s1.Read(d1, 0, length) != length) return false;
            if (s2.Read(d2, 0, length) != length) return false;
            for (int i = 0; i < length; i++)
            {
                if (d1[i] != d2[i])
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Parses a RIFF WAVE PCM header of a given input <see cref="Stream"/>
        /// </summary>
        /// <remarks>
        /// Upon succesful parsing the <paramref name="input"/> <see cref="Stream"/> is positioned at the beginning of the actual PCM data,
        /// that is at the beginning of the data field of the data sub-chunk
        /// </remarks>
        /// <param name="input">The input <see cref="Stream"/> - must be positioned at the start of the RIFF chunk</param>
        /// <returns>A <see cref="AudioLibPCMFormat"/> containing the parsed data</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when RIFF WAVE header is invalid or is not PCM data
        /// </exception>
        public static AudioLibPCMFormat RiffHeaderParse(Stream input, out uint dataLength)
        {
            System.Diagnostics.Debug.Assert(input.Position == 0);

            dataLength = 0;

            BinaryReader rd = new BinaryReader(input);

            //http://www.sonicspot.com/guide/wavefiles.html

            // Ensures 3x4=12 bytes available to read (RIFF Type Chunk)
            {
                long availableBytes = input.Length - input.Position;
                if (availableBytes < 12)
                {
                    throw new ArgumentOutOfRangeException(
                        "The RIFF chunk descriptor does not fit in the input stream");
                }
            }

            //Chunk ID (4 bytes)
            {
                string chunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
                if (chunkId != "RIFF")
                {
                    throw new ArgumentOutOfRangeException("ChunkId is not RIFF");
                }
            }
            //Chunk Data Size (the wavEndPos variable is used further below as the upper limit position in the stream)
            long wavEndPos = 0;
            {
                // 4 bytes
                uint chunkSize = rd.ReadUInt32();

                // Ensures the given size fits within the actual stream
                wavEndPos = input.Position + chunkSize;
                if (wavEndPos > input.Length)
                {
                    throw new ArgumentOutOfRangeException(String.Format(
                                                             "The WAVE PCM chunk does not fit in the input Stream (expected chunk end position is {0:0}, Stream count is {1:0})",
                                                             wavEndPos, input.Length));
                }
            }
            //RIFF Type (4 bytes)
            {
                string format = Encoding.ASCII.GetString(rd.ReadBytes(4));
                if (format != "WAVE")
                {
                    throw new ArgumentOutOfRangeException(String.Format(
                                                                       "RIFF format {0} is not supported. The only supported RIFF format is WAVE",
                                                                       format));
                }
            }


            // We need at least the 'data' and the 'fmt ' chunks
            bool foundWavDataChunk = false;
            bool foundWavFormatChunk = false;

            // We memorize the position of the actual PCM data in the stream,
            // so we can seek back, if needed (normally, this never happens as the 'data' chunk
            // is always the last one. However the WAV format does not mandate the order of chunks so...)
            long wavDataChunkPosition = -1;

            AudioLibPCMFormat pcmInfo = new AudioLibPCMFormat();

            //loop when there's at least 2x4=8 bytes to read (Chunk ID & Chunk Data Size)
            while (input.Position + 8 <= wavEndPos)
            {
                // 4 bytes
                string chunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));

                // 4 bytes
                uint chunkSize = rd.ReadUInt32();

                // Ensures the given size fits within the actual stream
                if (input.Position + chunkSize > wavEndPos)
                {
                    throw new ArgumentOutOfRangeException(String.Format(
                                                                       "ChunkId {0} does not fit in RIFF chunk",
                                                                       chunkId));
                }

                switch (chunkId)
                {
                    case "fmt ":
                        {
                            // The default information fields fit within 16 bytes
                            int extraFormatBytes = (int)chunkSize - 16;

                            // Compression code (2 bytes)
                            ushort compressionCode = rd.ReadUInt16();

                            // Number of channels (2 bytes)
                            ushort numChannels = rd.ReadUInt16();
                            if (numChannels == 0)
                            {
                                throw new ArgumentOutOfRangeException("0 channels of audio is not supported");
                            }

                            // Sample rate (4 bytes)
                            uint sampleRate = rd.ReadUInt32();

                            // Average bytes per second, aka byte-rate (4 bytes)
                            uint byteRate = rd.ReadUInt32();

                            // Block align (2 bytes)
                            ushort blockAlign = rd.ReadUInt16();

                            // Significant bits per sample, aka bit-depth (2 bytes)
                            ushort bitDepth = rd.ReadUInt16();

                            if (compressionCode != 0 && extraFormatBytes > 0)
                            {
                                // 	Extra format bytes (2 bytes)
                                uint extraBytes = rd.ReadUInt16();
                                if (extraBytes > 0)
                                {
                                    System.Diagnostics.Debug.Assert(extraFormatBytes == extraBytes);

                                    // Skip (we ignore the extra information in this chunk field)
                                    rd.ReadBytes((int)extraBytes);

                                    // check word-alignment
                                    if ((extraBytes % 2) != 0)
                                    {
                                        rd.ReadByte();
                                    }
                                }
                            }

                            if ((bitDepth % 8) != 0)
                            {
                                throw new ArgumentOutOfRangeException(String.Format(
                                                                                   "Invalid number of bits per sample {0:0} - must be a mulitple of 8",
                                                                                   bitDepth));
                            }
                            if (blockAlign != (numChannels * bitDepth / 8))
                            {
                                throw new ArgumentOutOfRangeException(String.Format(
                                                                                   "Invalid block align {0:0} - expected {1:0}",
                                                                                   blockAlign, numChannels * bitDepth / 8));
                            }
                            if (byteRate != sampleRate * blockAlign)
                            {
                                throw new ArgumentOutOfRangeException(String.Format(
                                                                                   "Invalid byte rate {0:0} - expected {1:0}",
                                                                                   byteRate, sampleRate * blockAlign));
                            }
                            pcmInfo.BitDepth = bitDepth;
                            pcmInfo.NumberOfChannels = numChannels;
                            pcmInfo.SampleRate = sampleRate;
                            pcmInfo.IsCompressed = compressionCode != 1;

                            foundWavFormatChunk = true;
                            break;
                        }
                    case "data":
                        {
                            if (input.Position + chunkSize > wavEndPos)
                            {
                                throw new ArgumentOutOfRangeException(String.Format(
                                                                                   "ChunkId {0} does not fit in RIFF chunk",
                                                                                   "data"));
                            }

                            dataLength = chunkSize;

                            foundWavDataChunk = true;
                            wavDataChunkPosition = input.Position;

                            // ensure we go past the PCM data, in case there are following chunks.
                            // (it's an unlikely scenario, but it's allowed by the WAV spec.)
                            input.Seek(chunkSize, SeekOrigin.Current);
                            break;
                        }
                    case "fact":
                        {
                            if (chunkSize == 4)
                            {
                                // 4 bytes
                                uint totalNumberOfSamples = rd.ReadUInt32();
                                // This value is unused, we are just reading it for debugging as we noticed that
                                // the WAV files generated by the Microsoft Audio Recorder
                                // contain the 'fact' chunk with this information. Most other recordings
                                // only contain the 'data' and 'fmt ' chunks.
                            }
                            else
                            {
                                rd.ReadBytes((int)chunkSize);
                            }
                            break;
                        }
                    case "wavl":
                    case "slnt":
                    case "cue ":
                    case "plst":
                    case "list":
                    case "labl":
                    case "note":
                    case "ltxt":
                    case "smpl":
                    case "inst":
                    default:
                        {
                            // Unsupported FOURCC codes, we skip.
                            rd.ReadBytes((int)chunkSize);
                            break;
                        }
                }
            }

            if (!foundWavDataChunk)
            {
                throw new ArgumentOutOfRangeException("WAV 'data' chunk was not found !");
            }
            if (!foundWavFormatChunk)
            {
                throw new ArgumentOutOfRangeException("WAV 'fmt ' chunk was not found !");
            }
            if (input.Position != wavDataChunkPosition)
            {
                input.Seek(wavDataChunkPosition, SeekOrigin.Begin);
            }

            return pcmInfo;
        }


        public ulong RiffHeaderWrite(Stream output, uint dataLength)
        {
            return RiffHeaderWrite(output, dataLength, NumberOfChannels, SampleRate, BitDepth);
        }

        private static ulong RiffHeaderWrite(Stream output,
            uint dataLength, ushort channels, uint sampleRate, ushort bitDepth)
        {
            ushort blockAlign = (ushort)(channels * bitDepth / 8);
            uint byteRate = channels * sampleRate * bitDepth / 8U;

            long initPos = output.Position;

            BinaryWriter wr = new BinaryWriter(output);
            wr.Write(Encoding.ASCII.GetBytes("RIFF")); //Chunk Uid
            uint chunkSize = 4 + 8 + 16 + 8 + dataLength;
            wr.Write(chunkSize); //Chunk Size
            wr.Write(Encoding.ASCII.GetBytes("WAVE")); //Format field
            wr.Write(Encoding.ASCII.GetBytes("fmt ")); //Format sub-chunk
            uint formatChunkSize = 16;
            wr.Write(formatChunkSize);
            ushort audioFormat = 1; //PCM format
            wr.Write(audioFormat);
            wr.Write(channels);
            wr.Write(sampleRate);
            wr.Write(byteRate);
            wr.Write(blockAlign);
            wr.Write(bitDepth);
            wr.Write(Encoding.ASCII.GetBytes("data"));
            wr.Write(dataLength);

            long endPos = output.Position;
            return (ulong)(endPos - initPos);
        }


        //public void CreateRIFF(BinaryWriter Writer)
        //{
        //    // Set up file with RIFF chunk info.
        //    char[] ChunkRiff = { 'R', 'I', 'F', 'F' };
        //    char[] ChunkType = { 'W', 'A', 'V', 'E' };
        //    char[] ChunkFmt = { 'f', 'm', 't', ' ' };
        //    char[] ChunkData = { 'd', 'a', 't', 'a' };
        //    short shPad = 1; // File padding
        //    int nFormatChunkLength = 0x10;
        //    // Format chunk length.
        //    short shBytesPerSample = 0;
        //    // Figure out how many bytes there will be per sample.
        //    if (8 == m_CaptureBuffer.Format.BitsPerSample && 1 == m_CaptureBuffer.Format.Channels)
        //        shBytesPerSample = 1;
        //    else if ((8 == m_CaptureBuffer.Format.BitsPerSample && 2 == m_CaptureBuffer.Format.Channels) || (16 == m_CaptureBuffer.Format.BitsPerSample && 1 == m_CaptureBuffer.Format.Channels))
        //        shBytesPerSample = 2;
        //    else if (16 == m_CaptureBuffer.Format.BitsPerSample && 2 == m_CaptureBuffer.Format.Channels)
        //        shBytesPerSample = 4;
        //    // Fill in the riff info for the wave file.
        //    Writer.Write(ChunkRiff);
        //    Writer.Write(1);
        //    Writer.Write(ChunkType);
        //    // Fill in the format info for the wave file.
        //    Writer.Write(ChunkFmt);
        //    Writer.Write(nFormatChunkLength);
        //    Writer.Write(shPad);
        //    Writer.Write(m_CaptureBuffer.Format.Channels);
        //    Writer.Write(m_CaptureBuffer.Format.SamplesPerSecond);
        //    Writer.Write(m_CaptureBuffer.Format.AverageBytesPerSecond);
        //    Writer.Write(shBytesPerSample);
        //    Writer.Write(m_CaptureBuffer.Format.BitsPerSample);
        //    // Now fill in the data chunk.
        //    Writer.BaseStream.Position = 36;
        //    Writer.Write(ChunkData);
        //    Writer.BaseStream.Position = 40;
        //    Writer.Write(0);	// The sample length will be written in later
        //    Writer.Close();
        //    //Writer = null;
        //}
    }
}
