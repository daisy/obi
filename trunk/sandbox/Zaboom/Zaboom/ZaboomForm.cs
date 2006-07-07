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
using UrakawaApplicationBackend.events.assetManagerEvents;

using urakawa.core;
using urakawa.media;

namespace Zaboom
{
    /// <summary>
    /// Main form for the application.
    /// Transport buttons and asset list.
    /// </summary>
    public partial class ZaboomForm : Form
    {
        private Project mProject;                 // the Urakawa project for this bit

        private AudioPlayer mPlayer;              // audio player instance
        private int mDeviceIndex;                 // index of the current output device in the list obtained from the player
        
        private AssetManager mManager;            // asset manager for this project
        private List<AudioMediaAsset> mPlayList;  // list of the audio assets to play
        
        private bool mPlayAll;                    // true if playing all assets, false if playing a single asset.

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
            // Create a new project object
            mProject = new Project();
            mProject.AppendMetadata("dc:Creator", Environment.UserName);
            mProject.AppendMetadata("dc:Date", DateTime.Now.ToString("yyyyMMdd"));
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
            mManager = new AssetManager(GetTemporaryDirectory());
            mPlayList = new List<AudioMediaAsset>();
            mPlayerStatusLabel.Text = mPlayer.State.ToString();
            renameAssetToolStripMenuItem.Enabled = false;
            deleteAssetToolStripMenuItem.Enabled = false;
            UpdateButtons();
            // Set up the events handler: on end of audio, move to the next asset; on player state change, refresh the buttons.
            mPlayer.EndOfAudioAsset.EndOfAudioAssetEvent +=
                new UrakawaApplicationBackend.events.audioPlayerEvents.DEndOfAudioAssetEvent(OnEndOfAsset);
            mPlayer.StateChangedEvent +=
                new UrakawaApplicationBackend.events.audioPlayerEvents.DStateChangedEvent(OnStateChanged);
            mManager.AssetRenamedEvent += new DAssetRenamedEvent(OnAssetRenamed);
            mManager.AssetDeletedEvent += new DAssetDeletedEvent(OnAssetDeleted);
        }

        /// <summary>
        /// Get a temporary directory to store the assets.
        /// </summary>
        /// <returns>The name of the project directory.</returns>
        private string GetTemporaryDirectory()
        {
            string path = Path.GetTempPath() + "\\zaboom-" + Path.GetRandomFileName();
            while (Directory.Exists(path)) path = Path.GetTempPath() + "\\" + Path.GetRandomFileName();
            Directory.CreateDirectory(path);
            Console.WriteLine("Directory for audio: " + path);
            return path;
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
                    if (asset.Validate())
                    {
                        asset = (AudioMediaAsset)mManager.CopyAsset(asset);
                        mManager.RenameAsset(asset, Path.GetFileNameWithoutExtension(path));
                        mAssetBox.Items.Add(asset.Name);
                        mPlayList.Add(asset);
                        if (mPlayer.State == AudioPlayerState.stopped) mAssetBox.SelectedIndex = mAssetBox.Items.Count - 1;
                        renameAssetToolStripMenuItem.Enabled = true;
                        deleteAssetToolStripMenuItem.Enabled = true;
                        // Create a node for this asset
                        mProject.AppendPhrase(asset);
                    }
                    else
                    {
                        MessageBox.Show("Unable to read audio file \"" + path + "\", format is not supported.",
                            "Unsupported audio format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                UpdateButtons();
            }
        }

        /// <summary>
        /// Save the project to a XUK file.
        /// </summary>
        private void saveXUKFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Urakawa project (*.xuk)|*.xuk";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Uri uri = new Uri(dialog.FileName);
                mProject.saveXUK(uri);
                Console.WriteLine("Saved project to {0}", uri);
            }
        }

        /// <summary>
        /// Quit the application.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mPlayer.State != AudioPlayerState.stopped) mPlayer.Stop();
            Directory.Delete(mManager.Directory, true);
            Application.Exit();
        }

        /// <summary>
        /// Rename the currently selected asset.
        /// </summary>
        private void renameAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mPlayList.Count > 0)
            {
                int selected = mAssetBox.SelectedIndex;
                AudioMediaAsset asset = mPlayList[selected];
                RenameAssetDialog dialog = new RenameAssetDialog(asset.Name);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mManager.RenameAsset(asset, dialog.AssetName);
                    mAssetBox.Items[selected] = asset.Name;
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
                mManager.DeleteAsset(mPlayList[selected]);
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
        /// Split the current asset at the current position. The first part keeps the same name and the second part is renamed.
        /// Ask for confirmation before splitting.
        /// For the moment split is only permitted while the asset is playing.
        /// TODO: play from the splitting point to test if the position is fine; fine tune the position.
        /// </summary>
        private void splitAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mAssetBox.SelectedIndex >= 0 && mPlayer.State == AudioPlayerState.paused)
            {
                int selected = mAssetBox.SelectedIndex;
                AudioMediaAsset asset = mPlayList[selected];
                SplitForm dialog = new SplitForm(asset, mPlayer);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ArrayList assets = asset.Split(mPlayer.CurrentTimePosition);
                    mPlayer.Stop();
                    mManager.DeleteAsset(asset);
                    mPlayList.RemoveAt(selected);
                    mAssetBox.Items.RemoveAt(selected);
                    AudioMediaAsset before = (AudioMediaAsset)assets[0];
                    mManager.AddAsset(before);
                    mPlayList.Insert(selected, before);
                    mManager.RenameAsset(before, asset.Name);
                    mAssetBox.Items.Insert(selected, before.Name);
                    AudioMediaAsset after = (AudioMediaAsset)assets[1];
                    mManager.AddAsset(after);
                    mPlayList.Insert(selected + 1, after);
                    mManager.RenameAsset(after, GetAfterName(asset.Name));
                    mAssetBox.Items.Insert(selected + 1, after.Name);
                    mAssetBox.SelectedIndex = selected + 1;
                    UpdateButtons();
                }
            }
        }

        /// <summary>
        /// Merge this asset with the following. Only when the player is stopped.
        /// </summary>
        private void mergeAssetWithNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mPlayer.State == AudioPlayerState.stopped && mAssetBox.SelectedIndex >= 0 &&
                mAssetBox.SelectedIndex < mAssetBox.Items.Count - 1 &&
                MessageBox.Show("Are you sure that you want to merge this asset with the following one?\nThis operation cannot be undone.",
                    "Merge the assets?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                int selected = mAssetBox.SelectedIndex;
                AudioMediaAsset merged = (AudioMediaAsset)mPlayList[selected].MergeWith(mPlayList[selected + 1]);
                merged.Name = mPlayList[selected].Name;
                mManager.DeleteAsset(mPlayList[selected]);
                mManager.DeleteAsset(mPlayList[selected + 1]);
                mManager.AddAsset(merged);
                mPlayList.RemoveRange(selected, 2);
                mPlayList.Insert(selected, merged);
                mAssetBox.Items.RemoveAt(selected + 1);
                mAssetBox.Items.RemoveAt(selected);
                mAssetBox.Items.Insert(selected, merged.Name);
                mAssetBox.SelectedIndex = selected;
                UpdateButtons();
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

        private void inputDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        /// <summary>
        /// Get a new name for an asset after a split.
        /// </summary>
        /// <param name="name">The original name</param>
        /// <returns>The new name (after split)</returns>
        private string GetAfterName(string name)
        {
            name = System.Text.RegularExpressions.Regex.Replace(name, @" \(after split(, #\d+)?\)$", "");
            string after = name + " (after split)";
            int i = 2;
            while (!mManager.IsAssetNameAvailable(after))
            {
                after = String.Format("{0} (after split, #{1})", name, i);
                ++i;
            }
            return after;
        }


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
        /// Catch renamed asset events.
        /// </summary>
        public void OnAssetRenamed(object sender, AssetRenamed e)
        {
        }

        /// <summary>
        /// Catch the deleted asset events.
        /// </summary>
        public void OnAssetDeleted(object sender, AssetDeleted e)
        {
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
                        splitAssetToolStripMenuItem.Enabled = true;
                        mergeAssetWithNextToolStripMenuItem.Enabled = false;
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
                        splitAssetToolStripMenuItem.Enabled = false;
                        mergeAssetWithNextToolStripMenuItem.Enabled = false;
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
                        splitAssetToolStripMenuItem.Enabled = false;
                        mergeAssetWithNextToolStripMenuItem.Enabled = mAssetBox.SelectedIndex < mAssetBox.Items.Count - 1;
                        break;
                }
            }
        }

        #region "buttons"

        private static readonly double MaxBackTime = 1000.0;  // max time for which we go back to the previous track

        /// <summary>
        /// Go backward: if we are at the very beginning of the asset, move to the previous asset, otherwise move back to the
        /// beginning of the asset.
        /// </summary>
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

        /// <summary>
        /// Move forward: start playing from the beginning of the next asset.
        /// </summary>
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

        #endregion

    }
}