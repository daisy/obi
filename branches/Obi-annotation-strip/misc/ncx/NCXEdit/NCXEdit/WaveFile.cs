using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCXEdit
{
    /// <summary>
    /// Store a wave file in memory and keep track of its format for playback.
    /// </summary>
    public class WaveFile
    {
        private string mFilePath;      // original path
        private string mLabel;         // label for this file, originally derived from the file name
        private WaveFormat mFormat;    // the format for this file
        private byte[] mBytes;         // for 8-bit samples, null for 16-bit data
        private short[] mShorts;       // for 16-bit samples, null for 8-bit data
        private uint mSamples;         // number of samples

        public string FilePath { get { return mFilePath; } }

        /// <summary>
        /// The label can be set/unset at will, but not completely erased.
        /// </summary>
        public string Label
        {
            get { return mLabel; }
            set { if (value != "") mLabel = value; }
        }

        public WaveFormat Format { get { return mFormat; } }
        public byte[] Bytes { get { return mBytes; } }
        public short[] Shorts { get { return mShorts; } }
        public uint Samples { get { return mSamples; } }

        /// <summary>
        /// Read in a new wave file. Throws an exception when we read something unexpected.
        /// </summary>
        /// <param name="path">the path to the wav file to read</param>
        public WaveFile(string path)
        {
            mFilePath = path;
            mFormat = null;
            FileInfo info = new FileInfo(path);
            mLabel = info.Name;
            mLabel = Path.ChangeExtension(mLabel, null);
            FileStream stream = info.OpenRead();
            BinaryReader reader = new BinaryReader(stream);
            uint bytesLeft = ReadHeader(reader);
            Console.WriteLine("{0} => {1} bytes to read", mLabel, bytesLeft);
            // Read the chunks. We only care about "fmt " and "data" chunk at the moment so we skip the rest.
            while (bytesLeft > 0)
            {
                ChunkHeader hd = ReadChunkHeader(reader);
                if (hd.id == "fmt ")
                {
                    if (mFormat != null)
                    {
                        throw new Exception("Incorrect file format: more than one format chunk?!");
                    }
                    mFormat = WaveFormat.Read(reader);
                    Console.WriteLine("#channels = {0}, sample rate = {1} Hz, bits/sample = {2}",
                        mFormat.Channels, mFormat.SampleRate, mFormat.BitsPerSample);
                }
                else if (hd.id == "data")
                {
                    if (mFormat == null)
                    {
                        // this is not incorrect but we don't know how to deal with that yet...
                        throw new Exception("Incorrect file format: data before format :(");
                    }
                    // we can now tell the number of samples
                    mSamples = (uint) (hd.size / (mFormat.BitsPerSample / 8) / mFormat.Channels);
                    hd.size = ReadData(reader, hd.size);  // kludgy
                }
                else
                {
                    Console.WriteLine("Skipping chunk \"{0}\" of size = {1}", hd.id, hd.size);
                    reader.BaseStream.Seek(hd.size, SeekOrigin.Current);
                }
                bytesLeft -= (hd.size + 8);
            }
            float length = mSamples / (float)mFormat.SampleRate;
            Console.WriteLine("Got {0} samples ({1}s).", mSamples, length);
            reader.Close();
        }

        /// <summary>
        /// Read the header of the file: must be RIFF/WAVE.
        /// </summary>
        /// <param name="reader">The current reader at the beginning of the file.</param>
        /// <returns>The size in bytes of the file (minus the first 12 bytes for this header.)</returns>
        private uint ReadHeader(BinaryReader reader)
        {
            ChunkHeader hd = ReadChunkHeader(reader);
            if (hd.id != "RIFF")
            {
                throw new Exception(String.Format("Unsupported file format: expected RIFF identifier, but got \"{0}\" instead.",
                    hd.id));
            }
            byte[] buf = reader.ReadBytes(4);
            if (Encoding.ASCII.GetString(buf) != "WAVE")
            {
                throw new Exception(String.Format("Unsupported file format: expected WAVE RIFF type, but got \"{0}\" instead.",
                    Encoding.ASCII.GetString(buf)));
            }
            return hd.size - 4;  // the size does not include the 8-byte header and we just read another 4 bytes
        }
        
        /// <summary>
        /// Read the actual data for the wave file and stores it either in a byte array (unsigned 8-bit samples)
        /// or a short array (signed 16-bits samples.) The other array is set to null.
        /// If there is a byte of padding at the end, then we have read one more byte than what was asked.
        /// </summary>
        /// <param name="reader">The current reader at the beginning of the data of the data chunk.</param>
        /// <param name="size">The number of bytes of data to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        private uint ReadData(BinaryReader reader, uint size)
        {
            if (mFormat.BitsPerSample == 8)
            {
                Console.Write("About to read {0} bytes...", size);
                mBytes = reader.ReadBytes((int)size);
                mShorts = null;
                // Mono files with 8-bit samples may have a byte of padding if the size is odd
                if (mFormat.Channels == 1 && size % 2 == 1)
                {
                    reader.BaseStream.Seek(1, SeekOrigin.Current);
                    Console.Write(" (skipped 1 byte of padding)");
                    ++size;
                }
            }
            else if (mFormat.BitsPerSample == 16)
            {
                Console.Write("About to read {0} bytes (but {1} shorts)...", size, size/2);
                mBytes = null;
                mShorts = new short[size / 2];
                for (int i = 0; i < size / 2; ++i)
                {
                    mShorts[i] = reader.ReadInt16();
                }
            }
            else
            {
                throw new Exception(String.Format("Unsupported file format: expected 8 or 16 bits per sample, but got {0} instead.",
                    mFormat.BitsPerSample));
            }
            Console.WriteLine(" done.");
            return size;
        }

        /// <summary>
        /// Every chunk has a 8-bytes header consisting of an identifier and a measure of its size.
        /// </summary>
        struct ChunkHeader
        {
            public string id;  // 4-char identifier, such as "RIFF", "fmt " or "data"
            public uint size;  // size of the chunk
        }

        /// <summary>
        /// Read the header from the upcoming chunk, and move the reader to the beginning of the data in the chunk.
        /// </summary>
        /// <param name="reader">The reader for the file at the beginning of the chunk.</param>
        /// <returns>The header for this chunk.</returns>
        private ChunkHeader ReadChunkHeader(BinaryReader reader)
        {
            ChunkHeader hd;
            byte[] buf = reader.ReadBytes(4);
            hd.id = Encoding.ASCII.GetString(buf);  // Get the header as a string for easy comparison
            hd.size = reader.ReadUInt32();
            return hd;
        }

        /// <summary>
        /// Store the format of a wave file (namely, number of channels, sample rate and bits per sample.)
        /// </summary>
        public class WaveFormat
        {
            public static ushort PCM = 1;   // value for PCM format, uncompressed (the only one we know right now)

            private ushort mChannels;       // number of channels
            private uint mSampleRate;       // sample rate in Hz
            private uint mByteRate;         // bytes/second
            private ushort mBlockAlign;     // block size
            private ushort mBitsPerSample;  // bits/sample

            public ushort Channels { get { return mChannels; } }
            public uint SampleRate { get { return mSampleRate; } }
            public uint ByteRate { get { return mByteRate; } }
            public ushort BlockAlign { get { return mBlockAlign; } }
            public ushort BitsPerSample { get { return mBitsPerSample; } }

            /// <summary>
            /// The constructor is not used directly, see Read() below.
            /// </summary>
            private WaveFormat() { }

            /// <summary>
            /// Read a PCM format header (16 bytes.) Throws an exception when it encounters an unknown format.
            /// </summary>
            /// <param name="reader">The current reader, at the beginning of the format chunk data.</param>
            /// <returns>A new WaveFormat instance.</returns>
            public static WaveFormat Read(BinaryReader reader)
            {
                WaveFormat fmt = new WaveFormat();
                ushort tag = reader.ReadUInt16();
                // We can only deal with PCM at the moment.
                if (tag != PCM)
                {
                    throw new Exception(String.Format("Unknown format tag: expected 1 (PCM) but got {0} instead.", tag));
                }
                fmt.mChannels = reader.ReadUInt16();
                fmt.mSampleRate = reader.ReadUInt32();
                fmt.mByteRate = reader.ReadUInt32();
                fmt.mBlockAlign = reader.ReadUInt16();
                fmt.mBitsPerSample = reader.ReadUInt16();
                return fmt;
            }
        }
    }
}
