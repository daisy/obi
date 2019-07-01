using System;
using System.Windows.Forms;
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
            var outPath = outputFileName + "NoiseREduction.wav";
            using (var reader = new AudioFileReader(open.FileName))
            {
               

                int BandPassFreqency = Int32.Parse(m_BandPassFrequencyTextBox.Text);
                // reader is the source for filter
                var filter = new MyWaveProvider(reader,BandPassFreqency);
                WaveFileWriter.CreateWaveFile16(outPath, filter);

                
            }

        }
    }
}
