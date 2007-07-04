using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.property.channel;

namespace Zaboom.UserControls
{
    public partial class TransportBar : UserControl
    {
        private Audio.Player mPlayer;            // audio player for this transport bar
        private List<AudioMediaData> mPlaylist;  // list of audio phrases to play
        private int mNowPlaying;                 // index in the playlist of the phrase currently playing

        /// <summary>
        /// Create a new transport bar.
        /// </summary>
        public TransportBar()
        {
            InitializeComponent();
            mPlayer = new Audio.Player();
            mPlayer.StateChanged += new Audio.StateChangedHandler(mPlayer_StateChanged);
            UpdatePlayerStatus();
            mPlaylist = new List<AudioMediaData>();
        }

        /// <summary>
        /// Get the parent as a project panel.
        /// </summary>
        private ProjectPanel ProjectPanel { get { return Parent as ProjectPanel; } }

        /// <summary>
        /// Update the playlist with the selection.
        /// </summary>
        private void ProjectPanel_SelectionChanged(ProjectPanel sender, EventArgs e)
        {
            mPlaylist.Clear();
            foreach (Selectable s in ProjectPanel.Selected)
            {
                if (s.Node != null)
                {
                    ChannelsProperty chprop = (ChannelsProperty)s.Node.getProperty(typeof(ChannelsProperty));
                    if (chprop != null)
                    {
                        ManagedAudioMedia media = chprop.getMedia(ProjectPanel.Project.AudioChannel) as ManagedAudioMedia;
                        if (media != null) mPlaylist.Add(media.getMediaData());
                    }
                }
            }
            UpdateButtonsState();
        }

        /// <summary>
        /// Play the currently selected phrase(s).
        /// </summary>
        private void mPlayButton_Click(object sender, EventArgs e)
        {
            if (mPlaylist.Count > 0)
            {
                mPlayer.EndOfAudioAsset += new Audio.EndOfAudioAssetHandler(mPlayer_EndOfAudioAsset);
                mNowPlaying = 0;
                Play();
            }
        }

        /// <summary>
        /// Play from the current item in the playlist.
        /// </summary>
        /// <returns>True if there was anything to play.</returns>
        private bool Play()
        {
            if (mNowPlaying < mPlaylist.Count)
            {
                mPlayer.Stop();
                mPlayer.Play(mPlaylist[mNowPlaying]);
                return true;
            }
            else
            {
                return false;
            }
        }         

        /// <summary>
        /// Move to the next asset in the playlist or stop if at the end.
        /// </summary>
        private void mPlayer_EndOfAudioAsset(Audio.Player player, EventArgs e)
        {
            ++mNowPlaying;
            if (!Play())
            {
                player.EndOfAudioAsset -= new Audio.EndOfAudioAssetHandler(mPlayer_EndOfAudioAsset);
                player.Stop();
            }
        }

        /// <summary>
        /// Update the transport bar when the player state changes.
        /// </summary>
        private void mPlayer_StateChanged(Audio.Player player, Audio.StateChangedEventArgs e)
        {
            UpdatePlayerStatus();
        }

        private delegate void VoidCallback();

        /// <summary>
        /// Update the player status in the transport bar.
        /// </summary>
        private void UpdatePlayerStatus()
        {
            if (mStatusLabel.InvokeRequired)
            {
                Invoke(new VoidCallback(UpdatePlayerStatus));
            }
            else
            {
                int w = mStatusLabel.Width;
                mStatusLabel.Text = mPlayer.State.ToString();
                mStatusLabel.Location = new Point(mStatusLabel.Location.X - mStatusLabel.Width + w, mStatusLabel.Location.Y);
                UpdateButtonsState();
            }
        }

        /// <summary>
        /// Update the state of the buttons depending on the player and selection state.
        /// </summary>
        private void UpdateButtonsState()
        {
            Enabled = mPlayer.State != Audio.PlayerState.NotReady;
            mPlayButton.Enabled = mPlayer.CanPlay && mPlaylist.Count > 0;
            mStopButton.Enabled = mPlayer.CanStop;
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        private void stopButton_Click(object sender, EventArgs e)
        {
            mPlayer.EndOfAudioAsset -= new Audio.EndOfAudioAssetHandler(mPlayer_EndOfAudioAsset);
            mPlayer.Stop();
        }

        /// <summary>
        /// Set the audio device and listen to selection events from the parent project panel.
        /// </summary>
        private void TransportBar_Load(object sender, EventArgs e)
        {
            ProjectPanel.SelectionChanged += new SelectionChangedHandler(ProjectPanel_SelectionChanged);
            mPlayer.SetDevice(ParentForm, "");
        }
    }
}