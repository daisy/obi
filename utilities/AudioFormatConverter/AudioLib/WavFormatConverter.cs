using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using NAudio.Wave;

namespace AudioLib
{
    public class WavFormatConverter : IWavFormatConverter
    {
        private bool m_OverwriteOutputFiles;

        public bool OverwriteOutputFiles
        {
            get { return m_OverwriteOutputFiles; }
            set { m_OverwriteOutputFiles = value; }
        }

        public WavFormatConverter(bool overwriteOutputFiles)
        {
            OverwriteOutputFiles = overwriteOutputFiles;
        }

        public string ConvertSampleRate(string sourceFile, string destinationDirectory,int destChannels ,int destSanplingRate, int destBitDepth)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            WaveStream sourceStream = null;
            WaveFormatConversionStream conversionStream = null;
            try
                {
                WaveFormat destFormat = new WaveFormat ( (int)destSanplingRate,
                                                                   destBitDepth,
                                                                   destChannels);
                sourceStream = new WaveFileReader ( sourceFile );

                conversionStream = new WaveFormatConversionStream ( destFormat, sourceStream );
                
                destinationFilePath = GenerateOutputFileFullname ( sourceFile, destinationDirectory, destChannels, destSanplingRate,destBitDepth);
                WaveFileWriter.CreateWaveFile ( destinationFilePath, conversionStream );
                }
                        finally
            {
                if (conversionStream != null)
                {
                    conversionStream.Close();
                }
                if (sourceStream != null)
                {
                    sourceStream.Close();
                }
            }

            return destinationFilePath;
        }

        private string GenerateOutputFileFullname(string sourceFile, string destinationDirectory, int destChannels, int destSamplingRate, int destBitDepth)
        {
            //FileInfo sourceFileInfo = new FileInfo(sourceFile);
            //string sourceFileName = sourceFileInfo.Name.Replace(sourceFileInfo.Extension, "");

            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            string sourceFileExt = Path.GetExtension(sourceFile);

            string channels = (destChannels == 1 ? "Mono" : (destChannels == 2 ? "Stereo" : destChannels.ToString()));

            string destFile = null;

            if (OverwriteOutputFiles)
            {
                destFile = Path.Combine(destinationDirectory,
                                           sourceFileName
                                           + "_"
                                           + destBitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + destSamplingRate
                                           + sourceFileExt);
            }
            else
            {
                Random random = new Random();

                int loopCounter = 0;
                do
                {
                    loopCounter++;
                    if (loopCounter > 10000)
                    {
                        throw new Exception("Not able to generate destination file name");
                    }
                    string randomStr = "_" + random.Next(100000).ToString();

                    destFile = Path.Combine(destinationDirectory,
                                        sourceFileName
                                        + "_"
                                        + destBitDepth
                                        + "-"
                                        + channels
                                        + "-"
                                        + destSamplingRate
                                        + randomStr
                                        + sourceFileExt);
                } while (File.Exists(destFile));
            }

            return destFile;
        }

        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public int ProgressInfo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string UnCompressWavFile ( string sourceFile, string destinationDirectory, int destChannels, int destSanplingRate, int destBitDepth )
        {
        throw new System.NotImplementedException ();

            // following code fails at ACM codec, so commented for now.
            /*
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            WaveStream sourceStream = null;
            WaveFormatConversionStream conversionStream = null;
            try
                {
                WaveFormat destFormat = new WaveFormat ( (int)destinationPCMFormat.SampleRate,
                                                                   destinationPCMFormat.BitDepth,
                                                                   destinationPCMFormat.NumberOfChannels );
                sourceStream = new WaveFileReader ( sourceFile );

                WaveStream intermediateStream = WaveFormatConversionStream.CreatePcmStream ( sourceStream );
                conversionStream = new WaveFormatConversionStream ( destFormat, intermediateStream);

                destinationFilePath = GenerateOutputFileFullname ( sourceFile, destinationDirectory, destinationPCMFormat );
                WaveFileWriter.CreateWaveFile ( destinationFilePath, conversionStream );
                }
                        finally
                {
                if (conversionStream != null)
                    {
                    conversionStream.Close ();
                    }
                if (sourceStream != null)
                    {
                    sourceStream.Close ();
                    }
                }
            return destinationFilePath;
             */ 
                    }

        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string UnCompressMp3File(string sourceFile, string destinationDirectory, int destChannels, int destSamplingRate, int destBitDepth)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            //PCMFormatInfo pcmFormat = null;
            int channels = 0;
            int sampleRate = 0;
            int bitDepth = 0;
            /*
            bool exceptionError = false;
            using (Mp3FileReader mp3Reader = new Mp3FileReader(sourceFile))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
                {
                    //pcmFormat = new PCMFormatInfo((ushort)pcmStream.WaveFormat.Channels,
                                                            //(uint)pcmStream.WaveFormat.SampleRate,
                                                            //(ushort)pcmStream.WaveFormat.BitsPerSample);
                channels = pcmStream.WaveFormat.Channels;
                sampleRate = pcmStream.WaveFormat.SampleRate;
                bitDepth = pcmStream.WaveFormat.BitsPerSample;
                    destinationFilePath = GenerateOutputFileFullname ( sourceFile + ".wav", destinationDirectory, pcmStream.WaveFormat.Channels, pcmStream.WaveFormat.SampleRate, pcmStream.WaveFormat.BitsPerSample );
                    using (WaveFileWriter writer = new WaveFileWriter(destinationFilePath, pcmStream.WaveFormat))
                    {
                        const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER  
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int byteRead;
                        try
                        {
                            writer.Flush();
                            while ((byteRead = pcmStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                writer.WriteData(buffer, 0, byteRead);
                            }
                        }
                        catch (Exception ex)
                        {
                            pcmStream.Close();
                            writer.Close();
                            //pcmFormat = null;
                            exceptionError = true;
                        }
                    }
                }
            }

            //if (pcmFormat == null)
            if ( exceptionError )
            {
                // in case of exception, delete incomplete file just created
            if (File.Exists ( destinationFilePath ))
                {
                File.Delete ( destinationFilePath );
                }
*/
                Stream fileStream = File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fileStream != null)
                {
                    int totalBytesWritten = 0;
                    using (NLayerMp3Stream mp3Stream = new NLayerMp3Stream(fileStream))
                    {
                        //pcmFormat = new PCMFormatInfo((ushort)mp3Stream.WaveFormat.Channels,
                                                      //(uint)mp3Stream.WaveFormat.SampleRate,
                                                      //(ushort)mp3Stream.WaveFormat.BitsPerSample);
                    channels = mp3Stream.WaveFormat.Channels;
                    sampleRate = mp3Stream.WaveFormat.SampleRate;
                    bitDepth = mp3Stream.WaveFormat.BitsPerSample;
                        destinationFilePath = GenerateOutputFileFullname ( sourceFile + ".wav", destinationDirectory, mp3Stream.WaveFormat.Channels, mp3Stream.WaveFormat.SampleRate, mp3Stream.WaveFormat.BitsPerSample );
                        using (WaveFileWriter writer = new WaveFileWriter(destinationFilePath, mp3Stream.WaveFormat))
                        {
                            int buffSize = mp3Stream.GetReadSize(4000);
                            //const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER  
                            byte[] buffer = new byte[buffSize];
                            try
                            {
                                int byteRead;
                                writer.Flush();
                                while ((byteRead = mp3Stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    Console.WriteLine("memory is " + (System.GC.GetTotalMemory(false) / 1024).ToString());
                                    writer.WriteData(buffer, 0, byteRead);
                                    totalBytesWritten += byteRead;
                                }
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                                System.Windows.Forms.MessageBox.Show(ex.ToString());
                                return null;
                            }
                        }
                    }
                    if (totalBytesWritten == 0)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            //}

            //if (!pcmFormat.IsCompatibleWith(destinationPCMFormat))
            if ( channels != destChannels
                || sampleRate != destSamplingRate 
                || bitDepth != destBitDepth )
            {
                string newDestinationFilePath =  ConvertSampleRate(destinationFilePath, destinationDirectory, destChannels, destSamplingRate, destBitDepth);
                if (File.Exists ( destinationFilePath ))
                    {
                    File.Delete ( destinationFilePath );
                    }
                return newDestinationFilePath;
            }
            return destinationFilePath;
        }

    }
}
