using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.WaveFormats;
using NAudio.Dsp;

namespace FirstTutorial
{
    public partial class ShowCueMarkers : Form
    {
        public ShowCueMarkers()
        {
            InitializeComponent();
        }

        private void m_btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Wave File (*.wav)|*.wav;";
            if (open.ShowDialog() != DialogResult.OK) return;
            string fileName = open.FileName;
            var inPath = fileName;
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "Cue.wav";
            //var reader = new AudioFileReader(inPath);
            CueWaveFileReader reader = new CueWaveFileReader(inPath);
            //var reader2 = new AudioFileReader(inPath);
            Cue cue = new Cue(9895490, "HI");
            if (reader.Cues != null)
            {
                //reader.Cues.Add(cue);


                MessageBox.Show(reader.Cues.Count.ToString());

                int[] list = reader.Cues.CuePositions;
                string[] cueLabelLists = reader.Cues.CueLabels;
                Console.WriteLine("List of cues  {0}", list);
                int cueLabelCount = 0;

                foreach (int cuePoint in list)
                {
                    decimal tempCuePointInSec = cuePoint / 44100;

                    MessageBox.Show("Cue Point Position in seconds : " + tempCuePointInSec.ToString() + " and it's label is " + cueLabelLists[cueLabelCount]);
                    cueLabelCount++;
                }

                //CueWaveFileWriter.CreateWaveFile(outPath, reader);
            }

        }
    }
}
