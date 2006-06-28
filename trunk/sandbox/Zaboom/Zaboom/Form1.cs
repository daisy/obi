using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using UrakawaApplicationBackend;

namespace Zaboom
{
    public partial class Form1 : Form
    {
        private AudioPlayer mPlayer;
        private AudioMediaAsset mAsset;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mPlayer = new AudioPlayer();
            ArrayList devices = mPlayer.GetOutputDevices();
            if (devices.Count == 0)
            {
                MessageBox.Show("Unable to find any audio device.\nWill close. Sorry.", "No audio device", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
            foreach (object device in devices)
            {
                mDeviceBox.Items.Add(device);
            }
            mDeviceBox.SelectedIndex = 0;
            mPlayer.SetDevice(this, 0);
            mAsset = null;
            mPlayButton.Enabled = false;
            mStopButton.Enabled = false;
        }

        private void mLoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Wave files (*.wav)|*.wav";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AudioMediaAsset asset = new AudioMediaAsset(dialog.FileName);
                mAssetBox.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
                mAsset = asset;
                mPlayButton.Enabled = true;
                mStopButton.Enabled = true;
            }
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            mPlayButton.Enabled = false;
            mPlayer.Play(mAsset);
        }

        private void Pause(object sender, EventArgs e)
        {
        }

        private void mStopButton_Click(object sender, EventArgs e)
        {

        }
    }
}