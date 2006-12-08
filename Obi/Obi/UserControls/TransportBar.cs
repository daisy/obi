using System;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// The transport bar: transport buttons, scrubbing slide, time display, text vu meter display.
    /// </summary>
    public partial class TransportBar : UserControl
    {
        private Playlist mPlaylist;          // current playlist (may be null)
        private ObiNode mPreviousSelection;  // selection before playback started

        // constants from the display combo box
        private static readonly int Elapsed = 0;
        private static readonly int ElapsedTotal = 1;
        private static readonly int Remain = 2;
        // private static readonly int RemainTotal = 3;

        /// <summary>
        /// Everything can be solved by adding a new layer of indirection. So here it is.
        /// </summary>
        public Audio.AudioPlayerState State
        {
            get { return mPlaylist == null ? Audio.AudioPlayerState.Stopped : mPlaylist.State; }
        }

        /// <summary>
        /// Predicate telling if play is possible.
        /// </summary>
        public bool CanPlay
        {
            get
            {
                return ((ProjectPanel)Parent).Project != null && Playlist.State == Audio.AudioPlayerState.Stopped;
            }
        }

        /// <summary>
        /// Predicate telling if resume is possible.
        /// </summary>
        public bool CanResume
        {
            get { return Playlist.State == Audio.AudioPlayerState.Paused; }
        }

        /// <summary>
        /// Whether recording is currently possible.
        /// </summary>
        public bool CanRecord
        {
            get
            {
                return (mPlaylist == null || mPlaylist.State == Audio.AudioPlayerState.Stopped) &&
                    ((ProjectPanel)Parent).SelectedNode != null;
            }
        }

        /// <summary>
        /// Set the playlist to be handled by the transport bar.
        /// </summary>
        public Playlist Playlist
        {
            get
            {
                if (mPlaylist == null)
                {
                    Playlist = new Playlist(((ProjectPanel)Parent).Project, Audio.AudioPlayer.Instance);
                }
                return mPlaylist;
            }
            set
            {
                mPlaylist = value;
                if (value != null)
                {
                    mPlaylist.MovedToPhrase += new Playlist.MovedToPhraseHandler(Play_MovedToPhrase);
                    mPlaylist.StateChanged += new Events.Audio.Player.StateChangedHandler(Play_PlayerStateChanged);
                    mPlaylist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler(Play_PlayerStopped);
                    mPreviousSelection = ((ProjectPanel)Parent).SelectedNode;
                    mDisplayBox.SelectedIndex = mPlaylist.WholeBook ? ElapsedTotal : Elapsed;
                }
            }
        }

        /// <summary>
        /// Initialize the transport bar.
        /// </summary>
        public TransportBar()
        {
            InitializeComponent();
            mPlaylist = null;
            mDisplayBox.SelectedIndex = ElapsedTotal;
        }

        /// <summary>
        /// Handles selection of phrases in the strip manager; i.e. move to the selected phrase.
        /// </summary>
        void StripManager_Selected(object sender, Obi.Events.Node.SelectedEventArgs e)
        {
            // CoreNode phrase = (CoreNode)sender;
            // if (phrase.GetType() == Type.GetType("Obi.PhraseNode") && e.Selected)
            if (e.Selected)
            {
                PhraseNode phrase = sender as PhraseNode;
                if (phrase != null)
                {
                    System.Diagnostics.Debug.Print("!!! Selected phrase caught ({0})", Playlist.CurrentPhrase == phrase ? "same" : "new");
                    if (Playlist.CurrentPhrase != phrase) Playlist.CurrentPhrase = (PhraseNode)phrase;
                }
            }
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
            if (mPlaylist != null) mPlaylist.NavigateToPreviousSection();
        }

        private void mPrevPhraseButton_Click(object sender, EventArgs e)
        {
            PrevPhrase();
        }

        /// <summary>
        /// Move to or play the previous phrase.
        /// </summary>
        public void PrevPhrase()
        {
            Playlist.NavigateToPreviousPhrase();
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
                if (mPlaylist == null || mPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    PhraseNode phrase = Playlist.CurrentPhrase;
                    Playlist = new Playlist(((ProjectPanel)Parent).Project, Audio.AudioPlayer.Instance);
                    Playlist.CurrentPhrase = phrase;
                    mScrubTrackBar.Maximum = Convert.ToInt32(mPlaylist.TotalAssetTime / 50);
                    mScrubTrackBar.Value = mScrubTrackBar.Maximum / 2;
                    mCentreSliderEventEffect = true;
                    mVUMeterPanel.Enable = true;
                    mVUMeterPanel.PlayListObj = mPlaylist;
                }
                mPlaylist.Play();
            }
            else if (CanResume)
            {
                mPlaylist.Resume();
            }
        }

        /// <summary>
        /// Play a single node (phrase or section).
        /// </summary>
        public void Play(urakawa.core.CoreNode node)
        {
            if (CanPlay)
            {
                Playlist = new Playlist(Audio.AudioPlayer.Instance, node);
                mScrubTrackBar.Maximum = Convert.ToInt32(mPlaylist.TotalAssetTime / 50);
                mScrubTrackBar.Value = mScrubTrackBar.Maximum / 2;
                mCentreSliderEventEffect = true;
                mPlaylist.Play();
                mVUMeterPanel.Enable = true;
                mVUMeterPanel.PlayListObj = mPlaylist;
            }
            else if (CanResume)
            {
                mPlaylist.Resume();
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
            Playlist.Pause();
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
            if (CanRecord)
            {
                ObiNode selected = ((ProjectPanel)Parent).StripManager.SelectedNode;
                SectionNode section; // section in which we are recording
                int index;           // index from which we add new phrases in the aforementioned section
                if (selected is SectionNode)
                {
                    section = (SectionNode)selected;
                    index = section.PhraseChildCount;
                }
                else
                {
                    section = ((PhraseNode)selected).ParentSection;
                    index = ((PhraseNode)selected).Index;
                }
                Settings settings = ((ObiForm)ParentForm).Settings;
                RecordingSession session = new RecordingSession(((ProjectPanel)Parent).Project, Audio.AudioRecorder.Instance,
                    selected, settings.AudioChannels, settings.SampleRate, settings.BitDepth);
                // the following closures handle the various events sent during the recording session
                session.StartingPhrase += new Events.Audio.Recorder.StartingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        ((ProjectPanel)Parent).Project.StartRecordingPhrase(_e, section, index);
                    }
                );
                session.ContinuingPhrase += new Events.Audio.Recorder.ContinuingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        ((ProjectPanel)Parent).Project.ContinuingRecordingPhrase(_e, section, index);
                    }
                );
                session.FinishingPhrase += new Events.Audio.Recorder.FinishingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        ((ProjectPanel)Parent).Project.FinishRecordingPhrase(_e, section, index);
                    }
                );
                new Dialogs.TransportRecord(session).ShowDialog();
            }
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
                System.Diagnostics.Debug.Print("!!! Stopping again, unselect all.");
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
            Playlist.NavigateToNextPhrase();
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
            if (mPlaylist != null) mPlaylist.NavigateToNextSection();
        }

        /// <summary>
        /// Update the transport bar according to the player state.
        /// </summary>
        private void Play_PlayerStateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            if (mPlaylist.State == Audio.AudioPlayerState.Stopped)
            {
                mDisplayTimer.Stop();
                Play_PlayerStopped(this, null);
            }
            else if (mPlaylist.State == Audio.AudioPlayerState.Paused)
            {
                mDisplayTimer.Stop();
                mPauseButton.Visible = false;
                mPlayButton.Visible = true;
            }
            else if (mPlaylist.State == Audio.AudioPlayerState.Playing)
            {
                mPauseButton.Visible = true;
                mPlayButton.Visible = false;
                mDisplayTimer.Start();
            }
            UpdateTimeDisplay();
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
        private void Play_MovedToPhrase(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            ((ProjectPanel)Parent).StripManager.SelectedPhraseNode = e.Node;
            mCentreSliderEventEffect = false;
            mScrubTrackBar.Maximum = Convert.ToInt32(mPlaylist.TotalAssetTime / 50);
            mScrubTrackBar.Value = mScrubTrackBar.Maximum / 2;
            mCentreSliderEventEffect = true;
        }

        /// <summary>
        /// Periodically update the time display.
        /// </summary>
        private void mDisplayTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        /// <summary>
        /// Update the time display to show current time. Depends on the what kind of timing is selected.
        /// </summary>
        public void UpdateTimeDisplay()
        {
            if (mPlaylist != null && mPlaylist.State != Obi.Audio.AudioPlayerState.Stopped)
            {
                mTimeDisplayBox.Text =
                    mDisplayBox.SelectedIndex == Elapsed ?
                        FormatTime(mPlaylist.CurrentTimeInAsset) :
                    mDisplayBox.SelectedIndex == ElapsedTotal ?
                        FormatTime(mPlaylist.CurrentTime) :
                    mDisplayBox.SelectedIndex == Remain ?
                        "-" + FormatTime(mPlaylist.RemainingTimeInAsset) :
                        "-" + FormatTime(mPlaylist.RemainingTime);
            }
            else
            {
                mTimeDisplayBox.Text = FormatTime(0.0);
            }
        }

        /// <summary>
        /// Format the time string for friendlier display.
        /// </summary>
        /// <param name="time">The time in milliseconds.</param>
        /// <returns>The time in hh:mm:ss format (fractions of seconds are discarded.)</returns>
        private string FormatTime(double time)
        {
            int s = Convert.ToInt32(time / 1000.0);
            string str = (s % 60).ToString("00");
            int m = Convert.ToInt32(s / 60);
            str = (m % 60).ToString("00") + ":" + str;
            int h = m / 60;
            return h.ToString("00") + ":" + str;
        }

        /// <summary>
        /// Update the time display immediatly when the display mode changes.
        /// </summary>
        private void mDisplayBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }


        private void tmTrackBar_Tick(object sender, EventArgs e)
        {
            tmTrackBar.Enabled = false;

            mCentreSliderEventEffect = false;
            mScrubTrackBar.Value = mScrubTrackBar.Maximum / 2;
            mCentreSliderEventEffect = true;
            
            if ( mTrackBarTimeValue  < 0)
                mTrackBarTimeValue = 0;

            if ( mTrackBarTimeValue > mPlaylist.TotalAssetTime )
                mTrackBarTimeValue =  mPlaylist.TotalAssetTime ;

            mPlaylist.CurrentTimeInAsset = mTrackBarTimeValue;
            UpdateTimeDisplay();
        }

        private bool mCentreSliderEventEffect = false ;
        private double mTrackBarTimeValue;

        private void mScrubTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (mCentreSliderEventEffect == true)
            {
                tmTrackBar.Enabled = false;

                mTrackBarTimeValue = mPlaylist.CurrentTimeInAsset ;

                mTrackBarTimeValue = mTrackBarTimeValue + (( mScrubTrackBar.Value -( mScrubTrackBar.Maximum / 2 )  ) * 100 );
                tmTrackBar.Start();
            }


        }

        private void TransportBar_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                ((ProjectPanel)Parent).StripManager.Selected += new Obi.Events.SelectedHandler(StripManager_Selected);
            }
        }

        private void TransportBar_Load(object sender, EventArgs e)
        {
            
        }
    }
}
