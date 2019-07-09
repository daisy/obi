using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.WaveFormats;
using NAudio.Dsp;

namespace Obi.Audio
{
    public class AudioProcessing
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

        public string Normalize(string fileName)
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
                reader.Volume = 1.0f / max;

                // write out to a new WAV file
                WaveFileWriter.CreateWaveFile16(outPath, reader);
                return outPath;
            }
        }
        
        public enum AudioProcessingKind { Amplify, FadeIn, FadeOut, Normalize, SoundTouch } ;
    }
}
