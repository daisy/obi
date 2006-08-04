using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WaveBits
{
    public partial class BitsForm : Form
    {
        private WaveFile mWaveFile = null;

        private WaveFmt mFormat = null;
        private WaveStream mAudioStream = null;
        private WaveOutPlayer mPlayer = null;

        public BitsForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Wave file|*.wav|Any file|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    mWaveFile = new WaveFile(openFileDialog1.FileName);
                    wavePanel.WaveFile = mWaveFile;
                    toolStripStatusLabel1.Text = mWaveFile.Label;
                    playToolStripMenuItem.Enabled = true;

                    WaveStream S = new WaveStream(openFileDialog1.FileName);
                    if (S.Length <= 0)
                        throw new Exception("Invalid WAV file");
                    mFormat = S.Format;
                    if (mFormat.wFormatTag != (short)WaveFmts.Pcm && mFormat.wFormatTag != (short)WaveFmts.Float)
                        throw new Exception("Olny PCM files are supported");
                    mAudioStream = S;
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Error opening file", MessageBoxButtons.OK);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Ready";
            playToolStripMenuItem.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Play();
        }

        private void Play()
        {
            Stop();
            if (mAudioStream != null)
            {
                mAudioStream.Position = 0;
                mPlayer = new WaveOutPlayer(-1, mFormat, 16384, 3, new BufferFillEventHandler(Filler));
            }
        }

        private void Filler(IntPtr data, int size)
        {
            if (mAudioStream != null)
            {
                if (mAudioStream.Position < mAudioStream.Length - 1)
                {
                    byte[] b = new byte[size];
                    mAudioStream.Read(b, 0, size);
                    System.Runtime.InteropServices.Marshal.Copy(b, 0, data, size);
                }
                else
                {
                    toolStripStatusLabel1.Text = "Stop!";
                    Stop();
                }
            }
        }

        private void __Filler(IntPtr data, int size)
        {
            byte[] b = new byte[size];
            if (mAudioStream != null)
            {
                int pos = 0;
                while (pos < size)
                {
                    int toget = size - pos;
                    int got = mAudioStream.Read(b, pos, toget);
                    if (got < toget)
                        mAudioStream.Position = 0; // loop if the file ends
                    pos += got;
                }
            }
            else
            {
                for (int i = 0; i < b.Length; i++)
                    b[i] = 0;
            }
            System.Runtime.InteropServices.Marshal.Copy(b, 0, data, size);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            if (mPlayer != null)
                try
                {
                    mPlayer.Dispose();
                }
                finally
                {
                    mPlayer = null;
                }
        }
    }
}