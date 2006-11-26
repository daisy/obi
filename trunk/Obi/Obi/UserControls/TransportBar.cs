using System;
using System.Windows.Forms;

namespace Obi.UserControls
{
    /// <summary>
    /// The transport bar: transport buttons, scrubbing slide, time display, text vu meter display.
    /// </summary>
    public partial class TransportBar : UserControl
    {
        private Playlist mPlaylist;
        private RecordingSession mRecordingSession;

        /// <summary>
        /// Set the playlist to be handled by the transport bar.
        /// </summary>
        public Playlist Playlist
        {
            set
            {
                mPlaylist = value;
                mPlaylist.MovedToPhrase += new Playlist.MovedToPhraseHandler(Play_MovedToPhrase);
                mPlaylist.StateChanged += new Events.Audio.Player.StateChangedHandler(Play_PlayerStateChanged);
                mPlaylist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler(Play_PlayerStopped);
            }
        }

        /// <summary>
        /// Set the recording session to be handled by the transport bar.
        /// </summary>
        public RecordingSession RecordingSession
        {
            set { mRecordingSession = value; }
        }

        /// <summary>
        /// Initialize the transport bar.
        /// </summary>
        public TransportBar()
        {
            InitializeComponent();
            mPlaylist = null;
            mRecordingSession = null;
        }

        private void mPrevSectionButton_Click(object sender, EventArgs e)
        {
            PrevSection();
        }

        /// <summary>
        /// Move to the previous section (i.e. first phrase of the previous section.)
        /// </summary>
        private void PrevSection()
        {
            if (mPlaylist != null) mPlaylist.NavigatePreviousSection();
        }

        private void mPrevPhraseButton_Click(object sender, EventArgs e)
        {
            PrevPhrase();
        }

        /// <summary>
        /// Play the previous phrase.
        /// </summary>
        public void PrevPhrase()
        {
            if (mPlaylist != null) mPlaylist.NavigatePreviousPhrase();
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            Play();
        }

        /// <summary>
        /// Play or resume.
        /// </summary>
        public void Play()
        {
            if (mPlaylist == null && ((ProjectPanel)Parent).Project != null)
            {
                Playlist = new Playlist(((ProjectPanel)Parent).Project, Audio.AudioPlayer.Instance);
            }
            if (mPlaylist != null) mPlaylist.Play();
        }

        private void mPauseButton_Click(object sender, EventArgs e)
        {
            Pause();
        }

        /// <summary>
        /// The pause button.
        /// </summary>
        public void Pause()
        {
            if (mPlaylist != null) mPlaylist.Pause();
        }

        private void mRecordButton_Click(object sender, EventArgs e)
        {

        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// The stop button
        /// </summary>
        public void Stop()
        {
            if (mPlaylist != null) mPlaylist.Stop();
        }

        private void mNextPhrase_Click(object sender, EventArgs e)
        {
            NextPhrase();
        }

        /// <summary>
        /// Go to the next phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (mPlaylist != null) mPlaylist.NavigateNextPhrase();
        }

        private void mNextSectionButton_Click(object sender, EventArgs e)
        {
            NextSection();
        }

        /// <summary>
        /// Move to the next section (i.e. the first phrase of the next section)
        /// </summary>
        public void NextSection()
        {
            if (mPlaylist != null) mPlaylist.NavigateNextSection();
        }

        /// <summary>
        /// Update the transport bar according to the player state.
        /// </summary>
        private void Play_PlayerStateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped)
            {
                Play_PlayerStopped(this, null);
            }
            else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mPauseButton.Visible = false;
                mPlayButton.Visible = true;
            }
            else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
            {
                mPauseButton.Visible = true;
                mPlayButton.Visible = false;
            }
        }

        /// <summary>
        /// Update the transport bar once the player has stopped.
        /// </summary>
        private void Play_PlayerStopped(object sender, EventArgs e)
        {
            mPauseButton.Visible = false;
            mPlayButton.Visible = true;
        }

        /// <summary>
        /// Highlight (i.e. select) the phrase currently playing.
        /// </summary>
        private void Play_MovedToPhrase(object sender, Events.Node.NodeEventArgs e)
        {
            ((ProjectPanel)Parent).StripManager.SelectedPhraseNode = e.Node;
        }
    }
}
