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
            mPlaylist.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler
            (
                delegate(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e) { PlayerStateChanged(); }
            );
            mPlaylist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler
            (
                delegate(object sender, EventArgs e) { PlayerStopped(); }
            );
            mPlaylist.MovedToPhrase += new Playlist.MovedToPhraseHandler(MovedToPhrase);
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
                if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    PlayerStopped();
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

        private void PlayerStopped()
        {
            mPauseButton.Visible = false;
            mPlayButton.Visible = true;
            mStopButton.Visible = false;
            mCloseButton.Visible = true;
        }

        private void MovedToPhrase(object sender, Events.Node.NodeEventArgs e)
        {
            System.Diagnostics.Debug.Print(">>> Moved to phrase {0}", Project.GetAudioMediaAsset(e.Node).Name);
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

        private void TransportPlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            mPlaylist.Stop();
        }

        private void btnNextPhrase_Click(object sender, EventArgs e)
        {
            mPlaylist.NavigateNextPhrase();
        }

        private void btnPreviousPhrase_Click(object sender, EventArgs e)
        {
            mPlaylist.NavigatePreviousPhrase();
        }
    }
}