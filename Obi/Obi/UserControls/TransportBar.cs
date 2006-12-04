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

        private urakawa.core.CoreNode mPreviousSelection;  // selection before playback started

        /// <summary>
        /// Everything can be solved by adding a new layer of indirection. So here it is.
        /// </summary>
        public Audio.AudioPlayerState State
        {
            get
            {
                return mPlaylist == null ? Audio.AudioPlayerState.Stopped : mPlaylist.State;
            }
        }

        /// <summary>
        /// Predicate telling if play is possible.
        /// </summary>
        public bool CanPlay
        {
            get
            {
                return ((ProjectPanel)Parent).Project != null &&
                    (mPlaylist == null ||
                    mPlaylist.State == Obi.Audio.AudioPlayerState.Stopped ||
                    mPlaylist.State == Obi.Audio.AudioPlayerState.Paused);
            }
        }

        /// <summary>
        /// Set the playlist to be handled by the transport bar.
        /// </summary>
        public Playlist Playlist
        {
            get { return mPlaylist; }
            set
            {
                mPlaylist = value;
                if (value != null)
                {
                    mPlaylist.MovedToPhrase += new Playlist.MovedToPhraseHandler(Play_MovedToPhrase);
                    mPlaylist.StateChanged += new Events.Audio.Player.StateChangedHandler(Play_PlayerStateChanged);
                    mPlaylist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler(Play_PlayerStopped);
                    mPreviousSelection = ((ProjectPanel)Parent).SelectedNode;
                }
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
            mDisplayBox.SelectedIndex = 0;
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
        /// <remarks>Create a new playlist everytime we start playing. We could be smarter about this.</remarks>
        public void Play()
        {
            if (CanPlay)
            {
                if (((ProjectPanel)Parent).Project != null &&
                    (mPlaylist == null || mPlaylist.State == Obi.Audio.AudioPlayerState.Stopped))
                {
                    Playlist = new Playlist(((ProjectPanel)Parent).Project, Audio.AudioPlayer.Instance);
                    mVUMeterPanel.PlayListObj = mPlaylist;
                }
                if (((ProjectPanel)Parent).SelectedNode != null)
                {
                    mPlaylist.CurrentPhrase = ((ProjectPanel)Parent).SelectedNode;
                    mPlaylist.Play(false);
                }
                else
                {
                    mPlaylist.Play();
                }
            }
        }

        /// <summary>
        /// Play a single node (phrase or section).
        /// </summary>
        public void Play(urakawa.core.CoreNode node)
        {
            if (CanPlay)
            {
                Playlist = new Playlist(((ProjectPanel)Parent).Project, Audio.AudioPlayer.Instance, node);
                mPlaylist.Play();
                mVUMeterPanel.PlayListObj = mPlaylist;
            }
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
            Record();
        }

        /// <summary>
        /// Start listening/recording.
        /// </summary>
        public void Record()
        {
            Settings CurrentSettings = Settings.GetSettings();
            urakawa.core.CoreNode node= null ; // Blank node for now, to be replaced by actual node  
            mRecordingSession = new RecordingSession ( ( ( ProjectPanel ) Parent ).Project , Audio.AudioRecorder.Instance  , node , CurrentSettings.AudioChannels , CurrentSettings.SampleRate , CurrentSettings.BitDepth ) ;
            Dialogs.TransportRecord TransportRecordDialog = new Obi.Dialogs.TransportRecord ( mRecordingSession ) ;
            TransportRecordDialog.Show ()  ;

        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// The stop button. Stopping twice deselects.
        /// </summary>
        public void Stop()
        {
            if (State == Obi.Audio.AudioPlayerState.Stopped)
            {
                ((ProjectPanel)Parent).StripManager.SelectedNode = null;
            }
            else if (mPlaylist != null)
            {
                mPlaylist.Stop();
            }
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
                mDisplayTimer.Stop();
                mTimeDisplayBox.Text = "00:00:00";
                Play_PlayerStopped(this, null);
            }
            else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mDisplayTimer.Stop();
                mPauseButton.Visible = false;
                mPlayButton.Visible = true;
            }
            else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
            {
                mPauseButton.Visible = true;
                mPlayButton.Visible = false;
                mDisplayTimer.Start();
            }
        }

        /// <summary>
        /// Update the transport bar once the player has stopped.
        /// </summary>
        private void Play_PlayerStopped(object sender, EventArgs e)
        {
            mPauseButton.Visible = false;
            mPlayButton.Visible = true;
            ((ProjectPanel)Parent).StripManager.SelectedNode = mPreviousSelection;
        }

        /// <summary>
        /// Highlight (i.e. select) the phrase currently playing.
        /// </summary>
        private void Play_MovedToPhrase(object sender, Events.Node.NodeEventArgs e)
        {
            ((ProjectPanel)Parent).StripManager.SelectedPhraseNode = e.Node;
        }

        private void mDisplayTimer_Tick(object sender, EventArgs e)
        {
            if (mPlaylist != null)
            {
                //int s = Convert.ToInt32(mPlaylist.CurrentTime / 1000.0);
                //string str = s.ToString("00");
                //int m = Convert.ToInt32(s / 60);
                //str = m.ToString("00") + ":" + str;
                //int h = m / 60;
                //mTimeDisplayBox.Text = h.ToString("00") + ":" + str;
                mTimeDisplayBox.Text = mPlaylist.CurrentTime.ToString();
            }
        }

        private void mDisplayBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
