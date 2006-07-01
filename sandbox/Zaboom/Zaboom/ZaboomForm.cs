using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Microsoft.DirectX.DirectSound;

using UrakawaApplicationBackend;
using UrakawaApplicationBackend.events.audioPlayerEvents;

namespace Zaboom
{
    /// <summary>
    /// Main form for the application.
    /// Transport buttons and asset list.
    /// </summary>
    public partial class ZaboomForm : Form
    {
        private AudioPlayer mPlayer;              // audio player instance
        private int mDeviceIndex;                 // index of the current output device in the list obtained from the player
        private List<AudioMediaAsset> mPlayList;  // list of the audio assets to play
        //private VuMeter mVuMeter;                 // VU Meter (not debug-proof, will readd later)

        private Dictionary<string, AudioMediaAsset> mPaths;  // imported files
        private bool mPlayAll;                               // true if playing all assets, false if playing a single asset.

        public ZaboomForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the form loads, create a player and set the output device to the first one by default.
        /// Set up the default event handlers.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the player and use the first output device by default.
            mPlayer = new AudioPlayer();
            ArrayList devices = mPlayer.GetOutputDevices();
            if (devices.Count == 0)
            {
                MessageBox.Show("Unable to find any audio device.\nWill close. Sorry.", "No audio device", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
            mDeviceIndex = 0;
            mPlayer.SetDevice(this, mDeviceIndex);
            mPlayList = new List<AudioMediaAsset>();
            mPaths = new Dictionary<string, AudioMediaAsset>();
            mPlayerStatusLabel.Text = mPlayer.State.ToString();
            renameAssetToolStripMenuItem.Enabled = false;
            deleteAssetToolStripMenuItem.Enabled = false;
            UpdateButtons();
            // Set up the events handler: on end of audio, move to the next asset; on player state change, refresh the buttons.
            mPlayer.EndOfAudioAsset.EndOfAudioAssetEvent +=
                new UrakawaApplicationBackend.events.audioPlayerEvents.DEndOfAudioAssetEvent(OnEndOfAsset);
            mPlayer.StateChangedEvent +=
                new UrakawaApplicationBackend.events.audioPlayerEvents.DStateChangedEvent(OnStateChanged);
            /*mVuMeter = new VuMeter();
            mPlayer.VuMeterObject = mVuMeter;
            mVuMeter.LowerThreshold = 50;
            mVuMeter.UpperThreshold = 83;
            mVuMeter.ScaleFactor = 2;
            mVuMeter.SampleTimeLength = 2000;
            mVuMeter.ShowForm();*/
        }

        #region "menu items"

        /// <summary>
        /// Add new assets to the list. A same path cannot be added twice.
        /// </summary>
        private void importAudioAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Wave files (*.wav)|*.wav|Any file|*.*";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string path in dialog.FileNames)
                {
                    AudioMediaAsset asset = new AudioMediaAsset(path);
                    if (mPaths.ContainsKey(path))
                    {
                        MessageBox.Show("Path \"" + path + "\" was already added.", "Duplicate path",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (asset.Validate())
                        {
                            mPaths.Add(path, asset);
                            asset.Name = Path.GetFileNameWithoutExtension(path);
                            mAssetBox.Items.Add(asset.Name);
                            mPlayList.Add(asset);
                            if (mPlayer.State == AudioPlayerState.stopped) mAssetBox.SelectedIndex = mAssetBox.Items.Count - 1;
                            renameAssetToolStripMenuItem.Enabled = true;
                            deleteAssetToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Unable to read audio file \"" + path + "\", format is not supported.",
                                "Unsupported audio format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                UpdateButtons();
            }
        }

        /// <summary>
        /// Quit the application.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mPlayer.State != AudioPlayerState.stopped) mPlayer.Stop();
            Application.Exit();
        }

        /// <summary>
        /// Rename the currently selected asset.
        /// </summary>
        private void renameAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mPlayList.Count > 0)
            {
                AudioMediaAsset asset = mPlayList[mAssetBox.SelectedIndex];
                RenameAssetDialog dialog = new RenameAssetDialog(asset.Name);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    asset.Name = dialog.AssetName;
                    mAssetBox.Items[mAssetBox.SelectedIndex] = asset.Name;
                }
            }
        }

        /// <summary>
        /// Delete the current asset from the list of assets
        /// TODO: clear the list when removing the last item?
        /// </summary>
        private void deleteAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mPlayer.State == AudioPlayerState.stopped && mAssetBox.SelectedIndex >= 0)
            {
                int selected = mAssetBox.SelectedIndex;
                string path = null;
                foreach (string p in mPaths.Keys)
                {
                    if (mPaths[p] == mPlayList[selected])
                    {
                        path = p;
                        break;
                    }
                }
                mPaths.Remove(path);
                mPlayList.RemoveAt(selected);
                mAssetBox.Items.RemoveAt(selected);
                if (mPlayList.Count > 0)
                {
                    mAssetBox.SelectedIndex = selected == mAssetBox.Items.Count ? selected - 1 : selected;
                }
                else
                {
                    renameAssetToolStripMenuItem.Enabled = false;
                    deleteAssetToolStripMenuItem.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Show a dialog to choose an output audio device.
        /// Only works when the audio player is stopped.
        /// </summary>
        private void outputDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mPlayer.State == AudioPlayerState.stopped)
            {
                OutputDeviceDialog dialog = new OutputDeviceDialog(mPlayer.GetOutputDevices(), mDeviceIndex);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mDeviceIndex = dialog.Selected;
                    mPlayer.SetDevice(this, mDeviceIndex);
                }
            }
        }

        #endregion

        /// <summary>
        /// Change the asset to be played. If we are playing, restart from this asset.
        /// </summary>
        private void mAssetBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedAssetChanged();
        }

        /// <summary>
        /// Play a different asset
        /// </summary>
        private void SelectedAssetChanged()
        {
            if (mPlayer.State == AudioPlayerState.playing)
            {
                mPlayer.Stop();
                mPlayer.Play(mPlayList[mAssetBox.SelectedIndex]);
            }
            else if (mPlayer.State == AudioPlayerState.paused)
            {
                mPlayer.Stop();
                mPlayer.Play(mPlayList[mAssetBox.SelectedIndex]);
                mPlayer.Pause();
            }
            UpdateButtons();
        }

        /// <summary>
        /// Update the status bar to show the current player state.
        /// </summary>
        public void OnStateChanged(object sender, StateChanged e)
        {
            UpdateStatusBar(e);
            UpdateButtons();
        }

        private delegate void UpdateStatusBarCallback(StateChanged e);

        /// <summary>
        /// Thread-safe modification of the status bar.
        /// </summary>
        /// <param name="e">The event from the audio player.</param>
        private void UpdateStatusBar(StateChanged e)
        {
            if (statusStrip1.InvokeRequired)
            {
                this.Invoke(new UpdateStatusBarCallback(UpdateStatusBar), new object[] { e });
            }
            else
            {
                mPlayerStatusLabel.Text = String.Format("{0} (was {1})", mPlayer.State.ToString(), e.OldState.ToString());
            }
        }

        public void OnEndOfAsset(object sender, EndOfAudioData e)
        {
            PlayNextAsset();
        }

        private delegate void PlayNextAssetCallback();

        /// <summary>
        /// Thread-safe method to play next asset if necessary and if it exists.
        /// The list of assets is changed to show the asset currently playing.
        /// </summary>
        private void PlayNextAsset()
        {
            if (mAssetBox.InvokeRequired)
            {
                Invoke(new PlayNextAssetCallback(PlayNextAsset));
            }
            else
            {
                if (mPlayAll && mAssetBox.SelectedIndex < mAssetBox.Items.Count - 1)
                {
                    ++mAssetBox.SelectedIndex;
                    mPlayer.Stop();
                    mPlayer.Play(mPlayList[mAssetBox.SelectedIndex]);
                }
            }
        }

        /// <summary>
        /// The play button act as play or pause.
        /// </summary>
        private void mPlayAllButton_Click(object sender, EventArgs e)
        {
            PlayAll();
        }

        /// <summary>
        /// Play all assets
        /// </summary>
        private void PlayAll()
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
                    mPlayAll = true;
                    if (mPlayList.Count > 0)
                    {
                        mAssetBox.SelectedIndex = 0;
                        mPlayer.Play(mPlayList[0]);
                    }
                    break;
            }
            UpdateButtons();
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            PlayOne();
        }

        /// <summary>
        /// Play all assets
        /// </summary>
        private void PlayOne()
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
                    mPlayAll = false;
                    if (mPlayList.Count > 0)
                    {
                        mPlayer.Play(mPlayList[mAssetBox.SelectedIndex]);
                    }
                    break;
            }
            UpdateButtons();
        }

        /// <summary>
        /// Stop playing. If we were playing all assets, go back to the beginning of the asset list.
        /// </summary>
        private void mStopButton_Click(object sender, EventArgs e)
        {
            mPlayAll = false;
            mPlayer.Stop();
            UpdateButtons();
        }




        private delegate void UpdateButtonsCallback();

        /// <summary>
        /// Thread-safe way to change the status of the buttons
        /// </summary>
        private void UpdateButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateButtonsCallback(UpdateButtons));
            }
            else
            {
                switch (mPlayer.State)
                {
                    case AudioPlayerState.paused:
                        if (mPlayAll)
                        {
                            mPlayAllButton.Text = "Pl&ay*";
                            mPlayAllButton.Enabled = true;
                            mPlayButton.Enabled = false;
                            mNextButton.Enabled = mAssetBox.SelectedIndex < mAssetBox.Items.Count - 1;
                        }
                        else
                        {
                            mPlayButton.Text = "&Play";
                            mPlayButton.Enabled = true;
                            mPlayAllButton.Enabled = false;
                            mNextButton.Enabled = false;
                        }
                        mPrevButton.Enabled = true;
                        mStopButton.Enabled = true;
                        outputDeviceToolStripMenuItem.Enabled = false;
                        break;
                    case AudioPlayerState.playing:
                        if (mPlayAll)
                        {
                            mPlayAllButton.Text = "P&ause";
                            mPlayAllButton.Enabled = true;
                            mPlayButton.Enabled = false;
                            mNextButton.Enabled = mAssetBox.SelectedIndex < mAssetBox.Items.Count - 1;
                        }
                        else
                        {
                            mPlayButton.Text = "&Pause";
                            mPlayButton.Enabled = true;
                            mPlayAllButton.Enabled = false;
                            mNextButton.Enabled = false;
                        }
                        mPrevButton.Enabled = true;
                        mStopButton.Enabled = true;
                        outputDeviceToolStripMenuItem.Enabled = false;
                        break;
                    case AudioPlayerState.stopped:
                        mPlayAllButton.Text = "Pl&ay*";
                        mPlayAllButton.Enabled = mPlayList.Count > 0;
                        mPlayButton.Text = "&Play";
                        mPlayButton.Enabled = mPlayList.Count > 0;
                        mStopButton.Enabled = false;
                        mPrevButton.Enabled = mAssetBox.SelectedIndex > 0;
                        mNextButton.Enabled = mAssetBox.SelectedIndex < mAssetBox.Items.Count - 1;
                        outputDeviceToolStripMenuItem.Enabled = true;
                        break;
                }
            }
        }

        private static readonly double MaxBackTime = 1000.0;  // max time for which we go back to the previous track

        private void mPrevButton_Click(object sender, EventArgs e)
        {
            switch (mPlayer.State)
            {
                case AudioPlayerState.paused:
                case AudioPlayerState.playing:
                    if (mPlayer.CurrentTimePosition < MaxBackTime && mPlayAll & mAssetBox.SelectedIndex > 0)
                    {
                        --mAssetBox.SelectedIndex;
                    }
                    SelectedAssetChanged();
                    break;
                case AudioPlayerState.stopped:
                    if (mAssetBox.SelectedIndex > 0)
                    {
                        --mAssetBox.SelectedIndex;
                        SelectedAssetChanged();
                    }
                    break;
            }
        }

        private void mNextButton_Click(object sender, EventArgs e)
        {
            switch (mPlayer.State)
            {
                case AudioPlayerState.paused:
                case AudioPlayerState.playing:
                    if (mPlayAll && mAssetBox.SelectedIndex <= mAssetBox.Items.Count - 1)
                    {
                        ++mAssetBox.SelectedIndex;
                        SelectedAssetChanged();
                    }
                    break;
                case AudioPlayerState.stopped:
                    if (mAssetBox.SelectedIndex <= mAssetBox.Items.Count - 1)
                    {
                        ++mAssetBox.SelectedIndex;
                        SelectedAssetChanged();
                    }
                    break;
            }
        }
    }
}