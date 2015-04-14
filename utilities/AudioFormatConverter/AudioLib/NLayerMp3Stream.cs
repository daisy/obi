using System.IO;
using NAudio.Wave;

//using NLayer.Decoder;
using javazoom.jl.decoder;

namespace AudioLib
{
    public class NLayerMp3Stream : WaveStream
    {
        private Stream source;
        Bitstream bitStream;
        WaveFormat waveFormat;
        Decoder decoder;
        WaveOutputBuffer outputBuffer;
        MemoryStream decodedStream;

        public NLayerMp3Stream(Stream source)
        {
            this.source = source;
            decoder = new Decoder(); //decoderParams);			
			bitStream = new Bitstream(new BackStream(source, 1024 * 10));
            
            Header header = bitStream.readFrame();
            int channels = (header.mode() == Header.SINGLE_CHANNEL) ? 1 : 2;
            int freq = header.frequency();
            int bits = 16;
            waveFormat = new WaveFormat(freq, bits, channels);
            decodedStream = new MemoryStream();
            outputBuffer = new WaveOutputBuffer(decodedStream, channels);
            source.Position = 0;

        }

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        long readPos;
        public override int Read(byte[] buffer, int offset, int count)
        {
            decodedStream.Position = decodedStream.Length;
            
            Header header = bitStream.readFrame();
            if (header == null)
                return 0;
            
            decoder.OutputBuffer = outputBuffer;
            Obuffer decoderOutput = decoder.decodeFrame(header, bitStream);
            bitStream.closeFrame();
            decodedStream.Position = readPos;

            int bytesRead = decodedStream.Read(buffer,offset,count);
            readPos += bytesRead;
            return bytesRead;
        }

        protected override void Dispose(bool disposing)
        {
            if (source != null)
            {
                source.Dispose();
                source = null;
            }
            base.Dispose(disposing);
        }

        public override long Length
        {
            get { return decodedStream.Length; }
        }

        public override long Position
        {
            get
            {
                return decodedStream.Position;
            }
            set
            {
                decodedStream.Position = 0;
            }
        }
    }

    class WaveOutputBuffer : Obuffer
    {
        BinaryWriter baseStream;
        private short[] buffer;
        private short[] bufferp;
        private int channels;

        public WaveOutputBuffer(Stream outputStream, int number_of_channels)
        {
            this.baseStream = new BinaryWriter(outputStream);
            buffer = new short[OBUFFERSIZE];
            bufferp = new short[MAXCHANNELS];
            channels = number_of_channels;

            clear_buffer();
        }

        /// <summary>
        /// Append a 16 bit PCM sample
        /// </summary>
        public override void append(int channel, short value)
        {
            buffer[bufferp[channel]] = value;
            bufferp[channel] += (short)channels;
        }

        /**
         * Write the samples to the file (Random Acces).
         */
        public override void write_buffer(int val)
        {

            for (int sample = 0; sample < bufferp[0]; sample++)
            {
                baseStream.Write(buffer[sample]);
            }

            clear_buffer();
        }

        public override void close()
        {
            baseStream.Close();
        }

        /**
         *
         */
        public override void clear_buffer()
        {
            for (int i = 0; i < channels; ++i) bufferp[i] = (short)i;
        }

        /**
         *
         */
        public override void set_stop_flag()
        { }


    }
}
