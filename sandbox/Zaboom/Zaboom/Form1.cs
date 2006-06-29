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
using UrakawaApplicationBackend.events.audioPlayerEvents;

namespace Zaboom
{
    public partial class Form1 : Form
    {
        private AudioPlayer mPlayer;
        // private VuMeter mVuMeter;
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
            mPlayer.EndOfAudioAsset.EndOfAudioAssetEvent +=
                new UrakawaApplicationBackend.events.audioPlayerEvents.DEndOfAudioAssetEvent(OnEndOfAudio);
            mPlayerStatusLabel.Text = mPlayer.State.ToString();
            mPlayer.StateChangedEvent +=
                new UrakawaApplicationBackend.events.audioPlayerEvents.DStateChangedEvent(OnStateChanged);
            // mVuMeter = new VuMeter();
            // mPlayer.VuMeterObject = mVuMeter;
            // mVuMeter.LowerThreshold = 50;
            // mVuMeter.UpperThreshold = 83;
            // mVuMeter.ScaleFactor = 2;
            // mVuMeter.SampleTimeLength = 2000;
            // mVuMeter.ShowForm();
        }

        private void mLoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Wave files (*.wav)|*.wav";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                UpdateStatusBar(StateChanged.From(mPlayer.State));
                mPlayer.Stop();
                AudioMediaAsset asset = new AudioMediaAsset(dialog.FileName);
                mAssetBox.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
                mAsset = asset;
                // mVuMeter.Channels = mAsset.Channels;
                mPlayButton.Enabled = true;
                mStopButton.Enabled = false;
            }
        }

        public void OnStateChanged(object sender, StateChanged e)
        {
            UpdateStatusBar(e);
        }

        private delegate void UpdateStatusBarCallback(StateChanged e);

        private void UpdateStatusBar(StateChanged e)
        {
            if (statusStrip1.InvokeRequired)
            {
                this.Invoke(new UpdateStatusBarCallback(UpdateStatusBar), new object[] { e });
            }
            else
            {
                UpdateButtons();
                mPlayerStatusLabel.Text = String.Format("{0} (was {1})", mPlayer.State.ToString(), e.OldState.ToString());
            }
        }

        public void OnEndOfAudio(object sender, EndOfAudioData e)
        {
            UpdateButtons();
        }

        private delegate void UpdateButtonsCallback();

        /// <summary>
        /// Thread-safe way to change the status of the buttons.
        /// </summary>
        private void UpdateButtons()
        {
            if (mPlayButton.InvokeRequired)
            {
                this.Invoke(new UpdateButtonsCallback(UpdateButtons));
            }
            else
            {
                switch (mPlayer.State)
                {
                    case AudioPlayerState.paused:
                        mPlayButton.Text = "&Play";
                        mStopButton.Enabled = true;
                        break;
                    case AudioPlayerState.playing:
                        mPlayButton.Text = "&Pause";
                        mStopButton.Enabled = true;
                        break;
                    case AudioPlayerState.stopped:
                        mPlayButton.Text = "&Play";
                        mStopButton.Enabled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// The play button act as play or pause.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPlayButton_Click(object sender, EventArgs e)
        {
            switch (mPlayer.State)
            {
                case AudioPlayerState.paused:
                    mPlayer.Resume();
                    break;
                case AudioPlayerState.playing:
                    mPlayer.Pause();
                    break;
                case AudioPlayerState.stopped:
                    mPlayer.Play(mAsset);
                    break;
            }
            UpdateButtons();
        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
            mPlayer.Stop();
            UpdateButtons();
        }
    }
}