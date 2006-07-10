using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;
using UrakawaApplicationBackend;
using UrakawaApplicationBackend.events.audioPlayerEvents;
using UrakawaApplicationBackend.events.assetManagerEvents;

namespace Zaboom
{
    public partial class Zaboom : Form
    {
        private AudioPlayer mPlayer;
        private Project mProject;

        public Zaboom()
        {
            InitializeComponent();
            // Initialize the player
            mPlayer = new AudioPlayer();
            ArrayList devices = mPlayer.GetOutputDevices();
            if (devices.Count == 0)
            {
                MessageBox.Show("Could not find any output device, will abort.", "No output device",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            mPlayer.SetDevice(this, 0);
            mPlayer.StateChangedEvent += new DStateChangedEvent(OnStateChangedEvent);
            OnStateChangedEvent(this, null);
            NoAsset();
            // Initialize the project
            AssetManager assmanager = new AssetManager(System.IO.Path.GetTempPath());
            assmanager.AssetRenamedEvent += new DAssetRenamedEvent(OnAssetRenamedEvent);
            mProject = new Project(assmanager);
        }

        private void NoAsset()
        {
            playToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            mPlayOneButton.Enabled = false;
            mStopButton.Enabled = false;
        }

        #region Menu Items

        private void importAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Audio file (*.wav)|*.wav|Any file|*.*";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in dialog.FileNames)
                {
                    string name = mProject.AddFile(filename);
                    mAssBox.Items.Add(name);
                    mAssBox.SelectedIndex = mAssBox.Items.Count - 1;
                    if (mAssBox.Items.Count == 1)
                    {
                        playToolStripMenuItem.Enabled = false;
                        stopToolStripMenuItem.Enabled = false;
                        mPlayOneButton.Enabled = true;
                        mStopButton.Enabled = true;
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XUK project (*.xuk)|*.xuk";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mProject.SaveXUK(dialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void renameAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mAssBox.SelectedIndex >= 0)
            {
                //NameDialog dialog = new NameDialog();
            }
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayOne();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopPlaying();
        }

        #endregion

        #region Buttons

        private void mPlayOneButton_Click(object sender, EventArgs e)
        {
            PlayOne();
        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
            StopPlaying();
        }

        #endregion

        #region Event handlers

        public void OnEndOfAudioStop(object sender, EndOfAudioData e)
        {
        }

        private delegate void UpdateOnStateChangedDelegate();

        private void UpdateOnStateChanged()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateOnStateChangedDelegate(UpdateOnStateChanged));
            }
            else
            {
                toolStripStatusLabel1.Text = "Audio player currently " + mPlayer.State + ".";
                playToolStripMenuItem.Text = mPlayOneButton.Text = mPlayer.State == AudioPlayerState.playing ? "&Pause" : "&Play";
                stopToolStripMenuItem.Enabled = mPlayer.State != AudioPlayerState.stopped;
            }
        }

        public void OnStateChangedEvent(object sender, StateChanged e)
        {
            UpdateOnStateChanged();
        }

        public void OnAssetRenamedEvent(object sender, AssetRenamed e)
        {
        }

        private void mAssBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mPlayer.State == AudioPlayerState.playing)
            {
                mPlayer.Stop();
                PlayOne();
            }
            else if (mPlayer.State == AudioPlayerState.paused)
            {
                mPlayer.Stop();
            }
        }

        #endregion

        /// <summary>
        /// Play the currently selected asset.
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
                    if (mAssBox.SelectedIndex >= 0)
                    {
                        mPlayer.EndOfAudioAsset.EndOfAudioAssetEvent += new DEndOfAudioAssetEvent(OnEndOfAudioStop);
                        mPlayer.Play(mProject.AssetAt(mAssBox.SelectedIndex));
                    }
                    break;
            }
        }

        /// <summary>
        /// Stop playing.
        /// </summary>
        private void StopPlaying()
        {
            if (mPlayer.State != AudioPlayerState.stopped) mPlayer.Stop();
        }
    }
}