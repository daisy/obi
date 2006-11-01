using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class TransportPlay : Form
    {
        private Playlist mPlaylist;

        public TransportPlay()
        {
            InitializeComponent();
        }

        public TransportPlay(Playlist playlist)
        {
            InitializeComponent();
            mPlaylist = playlist;
            mPlaylist.Audioplayer.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler(
                delegate(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e) { PlayerStateChanged(); }
            );
            mPlaylist.Play();
        }

        delegate void PlayerStateChangedCallback();

        private void PlayerStateChanged()
        {
            if (InvokeRequired)
            {
                Invoke(new PlayerStateChangedCallback(PlayerStateChanged));
            }
            else
            {
                System.Diagnostics.Debug.Print("NEW STATE = {0}", mPlaylist.Audioplayer.State);
                if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    mPauseButton.Visible = false;
                    mPlayButton.Visible = true;
                    mStopButton.Visible = false;
                    mCloseButton.Visible = true;
                }
                else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Paused)
                {
                    mPauseButton.Visible = false;
                    mPlayButton.Visible = true;
                    mStopButton.Visible = true;
                    mCloseButton.Visible = false;
                }
                else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
                {
                    mPauseButton.Visible = true;
                    mPlayButton.Visible = false;
                    mStopButton.Visible = true;
                    mCloseButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// Stops playback and close the dialog.
        /// </summary>
        private void mStopButton_Click(object sender, EventArgs e)
        {
            mPlaylist.Stop();
        }

        private void mPauseButton_Click(object sender, EventArgs e)
        {
            mPlaylist.Pause();
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            mPlaylist.Play();
        }
    }
}