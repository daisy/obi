﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.WaveFormats;
using NAudio.Dsp;

namespace FirstTutorial
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            m_IncreseAmplitudeComboBox.SelectedIndex = 0;
            m_DecreaseAmplitudeComboBox.SelectedIndex = 0;
        }

       // private NAudio.Wave.WaveFileReader wave = null;

     //   private NAudio.Wave.DirectSoundOut output = null;

        private string fileName = string.Empty;
        private OpenFileDialog open = new OpenFileDialog();
        private Process m_process = null;
        private CueWaveFileReader reader;
        private void button1_Click(object sender, EventArgs e)
        {
            
            open.Filter = "Wave File (*.wav)|*.wav;";
            if (open.ShowDialog() != DialogResult.OK) return;

            fileName = open.FileName;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            m_IncreaseAmplitude.Enabled = true;
            m_DecreaseAmplitudeButton.Enabled = true;
            FadeOutDurationButton.Enabled = true;
            FadeInDurationButton.Enabled = true;
            m_NoiseReductionButton.Enabled = true;
            m_ffmpegNoiseReduction.Enabled = true;

        }

       

     
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var inPath = fileName;
            //var outPath = @"H:\Repos\normalized1.wav";
            string outputFileName = fileName.Substring(0,fileName.Length - 4);
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
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //var outPath = @"H:\Repos\fadeIn2.wav";
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "FadeOut.wav";
            var reader = new AudioFileReader(open.FileName);
            TimeSpan span = reader.TotalTime;
            var fader = new FadeInOutSampleProvider(reader);

            double totalTime = span.TotalMilliseconds;
            fader.BeginFadeOut(totalTime);
            var stwp = new SampleToWaveProvider(fader);
            WaveFileWriter.CreateWaveFile(outPath, stwp); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //var outPath = @"H:\Repos\fadeIn2.wav";
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "FadeIn.wav";
            var reader = new AudioFileReader(open.FileName);
            TimeSpan span = reader.TotalTime;
            var fader = new FadeInOutSampleProvider(reader);
            double totalTime = span.TotalMilliseconds;
            fader.BeginFadeIn(totalTime);

            var stwp = new SampleToWaveProvider(fader);
            WaveFileWriter.CreateWaveFile(outPath, stwp); 
        }

        private void FadeOutDurationButton_Click(object sender, EventArgs e)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "FadeOutDuration.wav";
            var reader = new AudioFileReader(open.FileName);
            var fader = new DelayFadeOutSampleProvider(reader);

            double totalTime = Int32.Parse(FadeOutDurationTextBox.Text);
            double startingTime = Int32.Parse(FadeOutStartingPointTextBox.Text);
            fader.BeginFadeOut(startingTime, totalTime);
            var stwp = new SampleToWaveProvider(fader);
            WaveFileWriter.CreateWaveFile(outPath, stwp); 
        }

        private void FadeInDurationButton_Click(object sender, EventArgs e)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "FadeInDuration.wav";
            var reader = new AudioFileReader(open.FileName);
            var fader = new FadeInOutSampleProvider(reader);
            double totalTime = Int32.Parse(FadeInDurationTextBox.Text);
            fader.BeginFadeIn(totalTime);

            var stwp = new SampleToWaveProvider(fader);
            WaveFileWriter.CreateWaveFile(outPath, stwp); 
        }

        private void m_IncreaseAmplitude_Click(object sender, EventArgs e)
        {
            var inPath = fileName;
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var reader = new AudioFileReader(inPath);
            int SelectedItem = Int32.Parse(m_IncreseAmplitudeComboBox.SelectedItem.ToString());
            var outPath = outputFileName + "IncreaseAmplitude" + SelectedItem.ToString() + ".wav";
            reader.Volume = SelectedItem;
            // write out to a new WAV file
            WaveFileWriter.CreateWaveFile16(outPath, reader);
        }

        private void m_DecreaseAmplitudeButton_Click(object sender, EventArgs e)
        {
            var inPath = fileName;
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var reader = new AudioFileReader(inPath);
            float SelectedItem = float.Parse(m_DecreaseAmplitudeComboBox.SelectedItem.ToString());
            var outPath = outputFileName + "DecreaseAmplitude" + SelectedItem.ToString() +".wav";
            reader.Volume = SelectedItem;
            // write out to a new WAV file
            WaveFileWriter.CreateWaveFile16(outPath, reader);
        }

        private void m_NoiseReductionButton_Click(object sender, EventArgs e)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "NoiseReduction.wav";
            var outPathFinal = outputFileName + "NoiseReductionFinal.wav";
            using (var reader = new AudioFileReader(open.FileName))
            {
                int BandPassFreqency = Int32.Parse(m_BandPassFrequencyTextBox.Text);
                // reader is the source for filter
                var filterTemp = new MyWaveProvider(reader, BandPassFreqency);
                //WaveFileWriter.CreateWaveFile16(outPath, filter);
                //var tempReader = new AudioFileReader(outPath);
                //filter = new MyWaveProvider(tempReader, BandPassFreqency, true);
                 var filter = new MyWaveProvider(filterTemp, BandPassFreqency, true);
                WaveFileWriter.CreateWaveFile16(outPathFinal, filter);
                
            }
        }

        private void m_ffmpegNoiseReduction_Click(object sender, EventArgs e)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "ffmpegNoiseReduction.wav";
            using (var reader = new AudioFileReader(open.FileName))
            {
                string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
                if (!File.Exists(ffmpegPath))
                    throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Invalid source file path " + fileName);

          
                m_process = new Process();

                m_process.StartInfo.FileName = ffmpegPath;

                //m_process.StartInfo.RedirectStandardOutput = true;
                ////m_process.StartInfo.RedirectStandardError = false;
                //m_process.StartInfo.UseShellExecute = false;
                ////m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;


                //m_process.StartInfo.Arguments = string.Format("-i " + (char)34 + fileName + (char)34 + " -af " + (char)34 + "highpass=200, lowpass=3000" + (char)34 + " "+ (char)34 + outPath + (char)34);


                m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -af afftdn=nr=50:nf=-20 " + "\"" + outPath + "\"");
                //m_process.StartInfo.Arguments = string.Format("-i " + (char)34 + fileName + (char)34 + " -af " + (char)34 + "highpass=200, lowpass=3000" + (char)34 + " " + (char)34 + outPath + (char)34);


                m_process.Start();               
                m_process.WaitForExit();


                                

            }
        }

        private void m_MarkCue_Click(object sender, EventArgs e)
        {

            ShowCueMarkers dialogCueMarker = new ShowCueMarkers();
            dialogCueMarker.ShowDialog();
                       
        }

        private void m_WriteCue_Click(object sender, EventArgs e)
        {
            

            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Append);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
            char[] cue = new char[] { 'c', 'u', 'e', ' ' };
            bw.Write(cue, 0, 4); // "cue "
            bw.Write((int)28); // chunk size = 4 + (24 * # of cues)
            bw.Write((int)1); // # of cues
            // first cue point
            bw.Write((int)0); // unique ID of first cue
            bw.Write((int)0); // position
            char[] data = new char[] { 'd', 'a', 't', 'a' };
            bw.Write(data, 0, 4); // RIFF ID = "data"
            bw.Write((int)0); // chunk start
            bw.Write((int)0); // block start
            bw.Write((int)6895490); // sample offset - in a mono, 16-bits-per-sample WAV            
            bw.Close();
            fs.Dispose();
        }
    }
}
