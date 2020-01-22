using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.WaveFormats;
using NAudio.Dsp;
using System.IO;
using System.Diagnostics;

namespace Obi.ProjectView
{
    public class WavAudioProcessing
    {
        public string IncreaseAmplitude(string fileName, float processingFactor)
        {
            var inPath = fileName;
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + " IncreaseAmplitude" + ".wav";
            using (var reader = new AudioFileReader(inPath))
            {                
                reader.Volume = processingFactor;
                //// write out to a new WAV file
                WaveFileWriter.CreateWaveFile16(outPath, reader);
            }
            
            return outPath;
        }

        public string FadeIn(string fileName, double duration)
        {
            var inPath = fileName;
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "FadeInTemp.wav";
            using (var reader = new AudioFileReader(inPath))
            {
                //TimeSpan span = reader.TotalTime;
                var fader = new FadeInOutSampleProvider(reader, false);
                //double totalTime = span.TotalMilliseconds;
                fader.BeginFadeIn(duration);
                var stwp = new SampleToWaveProvider(fader);
                WaveFileWriter.CreateWaveFile(outPath, stwp);

            }

            using (var reader = new AudioFileReader(outPath))
            {
                outPath = outputFileName + "FadeIn.wav";
                //var temp = new Wave32To16Stream(reader);
                WaveFileWriter.CreateWaveFile16(outPath, reader);
            }
            return outPath;
        }

        public string FadeOut(string fileName, double duration)
        {
            var inPath = fileName;
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "FadeInTemp.wav";
            using (var reader = new AudioFileReader(inPath))
            {
                //TimeSpan span = reader.TotalTime;
                var fader = new FadeInOutSampleProvider(reader, false);
                //double totalTime = span.TotalMilliseconds;
                fader.BeginFadeOut(duration);
                var stwp = new SampleToWaveProvider(fader);
                WaveFileWriter.CreateWaveFile(outPath, stwp);

            }

            using (var reader = new AudioFileReader(outPath))
            {
                outPath = outputFileName + "FadeIn.wav";
                //var temp = new Wave32To16Stream(reader);
                WaveFileWriter.CreateWaveFile16(outPath, reader);
            }
            return outPath;
        }

        public string Normalize(string fileName, float processingFactor)
        {
            var inPath = fileName;
            //var outPath = @"H:\Repos\normalized1.wav";
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "normalized.wav";
            // var outPath = fileName + "\\ normalized.wav";
            float max = 0;

            using (var reader = new AudioFileReader(inPath))
            {
                // find the max peak
                float[] buffer = new float[reader.WaveFormat.SampleRate];
                int read;
                do
                {
                    read = reader.Read(buffer, 0, buffer.Length);
                    for (int n = 0; n < read; n++)
                    {
                        var abs = Math.Abs(buffer[n]);
                        if (abs > max) max = abs;
                    }
                } while (read > 0);
                Console.WriteLine("Max sample value: {0}", max);
                if (max == 0 || max > 1.0f)
                    throw new InvalidOperationException("File cannot be normalized");

                // rewind and amplify
                reader.Position = 0;
                //int SelectedItem = Int32.Parse(comboBox1.SelectedItem.ToString());
                //reader.Volume = SelectedItem;
                reader.Volume = (1.0f / max) * processingFactor;

                // write out to a new WAV file
                WaveFileWriter.CreateWaveFile16(outPath, reader);
                return outPath;
            }
        }

        public string  NoiseReductionNAudio(string fileName, float highPass, float LowPass)
        {
            
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "NoiseReduction.wav";
            using (var reader = new AudioFileReader(fileName))
            {
                // reader is the source for filter
                var filterTemp = new MyWaveProvider(reader,highPass);
                var filter = new MyWaveProvider(filterTemp, LowPass, true);
                WaveFileWriter.CreateWaveFile16(outPath, filter);
                return outPath;

            }
        }

        public string NoiseReductionFfmpeg(string fileName, float highPass, float LowPass)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "ffmpegNoiseReduction.wav";
            using (var reader = new AudioFileReader(fileName))
            {
                string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
                if (!File.Exists(ffmpegPath))
                    throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Invalid source file path " + fileName);

               
                Process m_process = new Process();

                m_process.StartInfo.FileName = ffmpegPath;

                m_process.StartInfo.RedirectStandardOutput = true;
                //m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = false;
                //m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //m_process.StartInfo.Arguments = string.Format("-i " + (char)34 + fileName + (char)34 + " -af " + (char)34 + "highpass=200, lowpass=3000" + (char)34 + " " + (char)34 + outPath + (char)34);

                m_process.StartInfo.Arguments = string.Format("-i " + (char)34 + fileName + (char)34 + " -af " + (char)34 + "highpass=" + highPass + ", lowpass=" + LowPass  + (char)34 + " " + (char)34 + outPath + (char)34);


                m_process.Start();
                m_process.WaitForExit();

                return outPath;



            }
        }

        public string NoiseReductionFfmpegAfftdn(string fileName, decimal noiseReductionVal, decimal noiseFloorVal)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "ffmpegNoiseReduction.wav";
            using (var reader = new AudioFileReader(fileName))
            {
                string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
                if (!File.Exists(ffmpegPath))
                    throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Invalid source file path " + fileName);


                Process m_process = new Process();

                m_process.StartInfo.FileName = ffmpegPath;

                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -af afftdn=nr=50:nf=-20 " + "\"" + outPath + "\"");
                m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -af afftdn=nr=" + noiseReductionVal +":nf="+ noiseFloorVal + " \"" + outPath + "\"");


                m_process.Start();
                m_process.WaitForExit();

                return outPath;



            }
        }

        public enum AudioProcessingKind { Amplify, FadeIn, FadeOut, Normalize, SoundTouch, NoiseReduction} ;
    }
}
