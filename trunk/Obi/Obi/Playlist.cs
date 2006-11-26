using Obi.Assets;
using Obi.Audio;
using System;
using System.Collections.Generic;
using urakawa.core;
using System.Windows.Forms;

namespace Obi
{
    
    /// <summary>
    /// The playlist is the list of phrases to be played. They are either the ones that were selected by the
    /// user, or just the list of phrases. The playlist knows how to play itself thanks to the application's
    /// audio player. It implements all the controls of the transport bar except for recording/start playing
    /// in a new context.
    /// </summary>
    public class Playlist
    {
        private Project mProject;         // project in which we are playing
        private AudioPlayer mPlayer;      // audio player for actually playing
        private List<CoreNode> mPhrases;  // list of phrase nodes (from which we get the assets)
        private int mCurrentPhraseIndex;  // index of the phrase currently playing
        private double mTotalTime;        // total time of this playlist
        private double mElapsedTime;      // elapsed time *before* the beginning of the current asset
        private bool mWholeBook;          // flag for playing whole book or just a selection
        private double mPausePosition= 0 ;
        private AudioPlayerState mPlayListState= AudioPlayerState.Stopped ;
        private VuMeter ob_VuMeter = new VuMeter();

        // Amount of time after which "previous phrase" goes to the beginning of the phrase
        // rather than the actual previous phrase. In milliseconds.
        private static readonly double InitialThreshold = 1500.0;

        // The playlist sends its own version of the state changed event in order to ignore spurrious
        // stop/start events sent by the audio player when moving between assets.
        // The VUmeter event should be caught as is; the end of asset should be ignored.
        public event Events.Audio.Player.StateChangedHandler StateChanged;

        // The end of the playlist was reached.
        public delegate void EndOfPlaylistHandler(object sender, EventArgs e);
        public event EndOfPlaylistHandler EndOfPlaylist;

        // Moved to a new phrase (while playing, or paused.)
        // this is to notify Obi to highlight the current phrase.
        public delegate void MovedToPhraseHandler(object sender, Events.Node.NodeEventArgs e);
        public event MovedToPhraseHandler MovedToPhrase;

        /// <summary>
        /// Get the audio player for the playlist. Useful for setting up event listeners.
        /// </summary>
        public AudioPlayer Audioplayer
        {
            get { return mPlayer; }
        }

        /// <summary>
        /// Get the next phrase in play order. Return null if we are at the end.
        /// </summary>
        public CoreNode NextPhrase
        {
            get { return mCurrentPhraseIndex < mPhrases.Count - 1 ? mPhrases[mCurrentPhraseIndex + 1] : null; }
        }

        /// <summary>
        /// Get the previous phrase in play order. Return the same phrase if this is the first phrase,
        /// or if we are past the initial threshold.
        /// </summary>
        public CoreNode PreviousPhrase
        {
            get
            {
                if (mCurrentPhraseIndex > 0)
                {
                    AudioMediaAsset current = mPlayer.CurrentAsset;
                    if (current == null) current = Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]);
                    // adapt the initial threshold to the length of the asset
                    double threshold = InitialThreshold > current.LengthInMilliseconds / 2.0 ?
                        InitialThreshold : current.LengthInMilliseconds / 2.0;
                    System.Diagnostics.Debug.Assert(mPlayer.State != AudioPlayerState.NotReady);
                    // really go back only if stopped or if before the end of the initial threshold
                    bool goback = mPlayer.State == AudioPlayerState.Stopped || mPlayer.CurrentTimePosition <= threshold;
                    return mPhrases[mCurrentPhraseIndex - (goback ? 1 : 0)];
                }
                else
                {
                    return mPhrases[0];
                }
            }
        }

        /// <summary>
        /// The section in which the currently playing phrase is.
        /// </summary>
        public CoreNode CurrentSection
        {
            get { return (CoreNode)mPhrases[mCurrentPhraseIndex].getParent(); }
        }

        /// <summary>
        /// First phrase of the next section (if any.) Defined only if the whole book is playing.
        /// </summary>
        public CoreNode NextSection
        {
            get
            {
                System.Diagnostics.Debug.Assert(mWholeBook);
                // instead of looking through the tree, just look at the playlist
                CoreNode current = CurrentSection;
                int i = mCurrentPhraseIndex + 1;
                for (; i < mPhrases.Count && mPhrases[i].getParent() == current; ++i) { }
                return i < mPhrases.Count ? mPhrases[i] : null;
            }
        }

        /// <summary>
        /// First phrase of the current section (if this is not the first phrase of the section),
        /// or of the previous section (
        /// </summary>
        public CoreNode PreviousSection
        {
            get
            {
                System.Diagnostics.Debug.Assert(mWholeBook);
                // TODO
                // if the current elapsed time in this section is less than the initial threshold,
                // return the first phrase of the previous section; otherwise, the first phrase of
                // the current section. See above for finding sections.
                return null;
            }
        }

        /// <summary>
        /// Get/set the current playing time inside the playlist in milliseconds.
        /// Setting the current time starts/continues playing from that position, or pauses at that position.
        /// </summary>
        public double CurrentTime
        {
            get
            {

                    return GetCurrentTime();
            }
            set
            {
                // TODO
                // check that the time is within bounds
                // find which phrase it happens in
                // if playing, resume to this position
                // if stopped, start playing from that position
                // if paused, move to that position but stay paused
                SetTimeInPlayList( value ) ;
            }
        }

        /// <summary>
        /// Get the total time for this playlist in milliseconds.
        /// </summary>
        public double TotalTime
        {
            get { return mTotalTime; }
        }

        /// <summary>
        /// Remaining time in the playlist in milliseconds.
        /// </summary>
        public double RemainingTime
        {
            get { return mTotalTime - CurrentTime; }
            set
            {
                // TODO
                // same as setting current time, but from the end!
            }
        }


        // functions start from here on

        /// <summary>
        /// Create a playlist for the whole project.
        /// The playlist contains all phrase nodes.
        /// </summary>
        /// <param name="project">The project for this playlist.</param>
        /// <param name="player">The audio player for this playlist.</param>
        public Playlist(Project project, AudioPlayer player)
        {
            InitPlaylist(project, player, project.RootNode, true);
            mPlayer.VuMeterObject = ob_VuMeter;
            
        }

        /// <summary>
        /// Create a playlist for a single (presumably selected) node.
        /// If the node is a phrase, add this only phrase to the playlist.
        /// If the node is a section, add all of its phrases to the playlist.
        /// </summary>
        /// <param name="project">The project for this playlist.</param>
        /// <param name="player">The audio player for this playlist.</param>
        /// <param name="node">The phrase or section node in the playlist.</param>
        public Playlist(Project project, AudioPlayer player, CoreNode node)
        {
            InitPlaylist(project, player, node, false);
            mPlayer.VuMeterObject = ob_VuMeter;
        }

        /// <summary>
        /// Initialize the playlist with the given node and project.
        /// </summary>
        /// <param name="project">The project for this playlist.</param>
        /// <param name="player">The audio player for this playlist.</param>
        /// <param name="node">The node from which to initialize the playlist.</param>
        /// <param name="wholeBook">Flag telling whether we are playing the whole book.</param>
        private void InitPlaylist(Project project, AudioPlayer player, CoreNode node, bool wholeBook)
        {
            mProject = project;
            mPlayer = player;
            mPhrases = new List<CoreNode>();
            mCurrentPhraseIndex = 0;
            mElapsedTime = 0.0;
            mTotalTime = 0.0;
            mWholeBook = wholeBook;
            node.visitDepthFirst
            (
                // Add all phrase nodes underneath (and including) the starting node.
                delegate(ICoreNode n)
                {
                    if (Project.GetNodeType((CoreNode)n) == NodeType.Phrase)
                    {
                        mPhrases.Add((CoreNode)n);
                        mTotalTime += Project.GetAudioMediaAsset((CoreNode)n).LengthInMilliseconds;
                    }
                    return true;
                },
                // nothing to do in post-visit
                delegate(ICoreNode n) { }
            );
            mPlayer.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(MoveToNextPhrase);
            mPlayer.StateChanged += new Events.Audio.Player.StateChangedHandler
            (
                // Intercept state change events from the player, and only pass along those that
                // involve the "not ready" state.
                delegate (object sender, Events.Audio.Player.StateChangedEventArgs e)
                {
                    if (e.OldState == AudioPlayerState.NotReady || mPlayer.State == AudioPlayerState.NotReady) 
                    {
                        StateChanged(this, e);
                    }
                }
            );
            System.Diagnostics.Debug.Print("Initialized playlist with {0} asset{1}; total time {2}ms.", mPhrases.Count,
                mPhrases.Count > 1 ? "s" : "", mTotalTime);
        }

        /// <summary>
        /// Start playing or resume playing after pausing.
        /// </summary>
        public void Play()
        {
            if (mPhrases.Count > 0)
            {
                //if ( mPausePosition != 0  || mPausedPhraseIndex != 0 )
                if (mPlayListState == AudioPlayerState.Paused)
                {
                    // Resume by using play from function
                    mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]) , mPausePosition );
                    mPlayListState = AudioPlayerState.Playing;
                    
                    if (StateChanged != null)
                    {
                        StateChanged(this, new Events.Audio.Player.StateChangedEventArgs(AudioPlayerState.Paused));
                    }
                }
                //else if ( mPausePosition == 0 && mPausedPhraseIndex == 0 )   
                else if (mPlayListState == AudioPlayerState.Stopped)
                {
                    // start from the beginning
                    mCurrentPhraseIndex = 0;
                    mElapsedTime = 0.0;
                    mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]));
                    mPlayListState = AudioPlayerState.Playing;
                    ob_VuMeter.ShowForm();
                    if (StateChanged != null)
                    {
                        StateChanged(this, new Events.Audio.Player.StateChangedEventArgs(AudioPlayerState.Stopped));
                    }
                    if (MovedToPhrase != null)
                    {
                        MovedToPhrase(this, new Events.Node.NodeEventArgs(this, mPhrases[mCurrentPhraseIndex]));
                    }
                }
                else
                {
                    throw new Exception(string.Format("Player should be paused or stopped, but is {0}.", mPlayer.State));
                }
            }
            else
            {
                // nothing to play so just stop.
                Stop();
            }
        }

        /// <summary>
        /// Catch the end of an asset from the audio player and move to the next phrase.
        /// </summary>
        /// <param name="sender">Sender of the event (i.e. the audio player.)</param>
        /// <param name="e">The arguments sent by the player.</param>
        private void MoveToNextPhrase(object sender, Events.Audio.Player.EndOfAudioAssetEventArgs e)
        {
            // add an option to have a beep between assets
            System.Media.SystemSounds.Exclamation.Play();

            if (mCurrentPhraseIndex < mPhrases.Count - 1)
            {
                PlayNextPhrase();
            }
            else if (EndOfPlaylist != null)
            {
                mPlayListState = AudioPlayerState.Stopped;
                ob_VuMeter.CloseVuMeterForm();
                EndOfPlaylist(this, new EventArgs());
            }
        }

        /// <summary>
        /// Play the next phrase in the list.
        /// </summary>
        private void PlayNextPhrase()
        {
            SkipToNextPhrase();
            Events.Audio.Player.StateChangedEventArgs evargs = new Events.Audio.Player.StateChangedEventArgs(mPlayer.State);
            mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]));
            mPlayListState = AudioPlayerState.Playing;
            // send the state change event if the state actually changed
            if (StateChanged != null && mPlayer.State != evargs.OldState) StateChanged(this, evargs);
        }

        /// <summary>
        /// Move to the next phrase in the list.
        /// </summary>
        private void SkipToNextPhrase()
        {
            mElapsedTime += mPlayer.CurrentAsset.LengthInMilliseconds;
            ++mCurrentPhraseIndex;
            
            if (MovedToPhrase != null)
            {
                MovedToPhrase(this, new Events.Node.NodeEventArgs(this, mPhrases[mCurrentPhraseIndex]));
            }
        }

        private void PlayPreviousPhrase()
        {
            SkipToPreviousPhrase();
            Events.Audio.Player.StateChangedEventArgs evargs = new Events.Audio.Player.StateChangedEventArgs(mPlayer.State);
            mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]));
            mPlayListState = AudioPlayerState.Playing;
            // send the state change event if the state actually changed
            if (StateChanged != null && mPlayer.State != evargs.OldState) StateChanged(this, evargs);
        }


        private void SkipToPreviousPhrase()
        {
            mElapsedTime -= mPlayer.CurrentAsset.LengthInMilliseconds;
            --mCurrentPhraseIndex;
            if (MovedToPhrase != null)
            {
                MovedToPhrase(this, new Events.Node.NodeEventArgs(this, mPhrases[mCurrentPhraseIndex]));
            }
        }


        /// <summary>
        /// Pause.
        /// </summary>
        public void Pause()
        {
            if (mPlayListState == AudioPlayerState.Playing)
            {
                System.Diagnostics.Debug.Assert(mPlayer.State == AudioPlayerState.Playing, "Can only pause while playing.");
                mPausePosition = mPlayer.CurrentTimePosition;
                mPlayer.Stop();
                mPlayListState = AudioPlayerState.Paused;
                if (StateChanged != null)
                {
                    StateChanged(this, new Events.Audio.Player.StateChangedEventArgs(AudioPlayerState.Playing));
                }
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            if (mPlayListState != AudioPlayerState.Stopped)
            {
                Events.Audio.Player.StateChangedEventArgs evargs = new Events.Audio.Player.StateChangedEventArgs(mPlayer.State);
                mPausePosition = 0;
                mPlayer.Stop();
                mPlayListState = AudioPlayerState.Stopped;
                if (StateChanged != null) StateChanged(this, evargs);
            }
        }

        // Navigation functions. The following applies to all functions:
        // If the player is playing, keep playing.
        // If the player is paused, stay paused.
        // If the player is stopped, start playing from the beginning of the next asset.

        /// <summary>
        /// Move to the next phrase.
        /// If there is no next phrase, immediatly stop.
        /// </summary>
        public void NavigateNextPhrase()
        {
            
            System.Diagnostics.Debug.Assert(mPlayer.State != AudioPlayerState.NotReady, "Player is not ready!");
            if (NextPhrase != null)  // current phrase is not last phrase
            {
                //if (mPlayer.State == AudioPlayerState.Playing)
                if (mPlayListState == AudioPlayerState.Playing)
                {
                    mPausePosition = 0;
                    mPlayer.Stop();
                    PlayNextPhrase();
                }
                //else if (mPlayer.State == AudioPlayerState.Stopped )
                else if (mPlayListState == AudioPlayerState.Paused)
                {
                    SkipToNextPhrase();
                    mPausePosition = 0;
                }
                //else if (mPlayer.State == AudioPlayerState.Stopped)
                else if (mPlayListState == AudioPlayerState.Stopped)
                {
                    PlayNextPhrase();
                }
            }
            else    // current phrase is last phrase
            {
                Stop();
            }
        }

        /// <summary>
        /// Move backward one phrase.
        /// If the current position is past the initial threshold, move back to the beginning of the current phrase.
        /// When there is no previous phrase, move to the beginning of the current phrase.
        /// </summary>
        public void NavigatePreviousPhrase()
        {
            // TODO
            if (mPlayer.CurrentTimePosition > InitialThreshold) 
            {
                // move play cursor to starting point of current phrase
                PlayFromStartOfPhrase();
            }
            else
            {
                // move play cursor to previous phrase
                System.Diagnostics.Debug.Assert(mPlayer.State != AudioPlayerState.NotReady, "Player is not ready!");
                if (mCurrentPhraseIndex != 0) // current phrase is not first phrase
                {
                    if (mPlayer.State == AudioPlayerState.Playing)
                    {
                        mPausePosition = 0;
                        mPlayer.Stop();
                        PlayPreviousPhrase();
                    }
                    //else if (mPlayer.State == AudioPlayerState.Stopped)
                    else if (mPlayListState == AudioPlayerState.Paused)
                    {
                        SkipToPreviousPhrase();
                        mPausePosition = 0;
                    }
                    else if (mPlayListState == AudioPlayerState.Stopped)
                    {
                        PlayPreviousPhrase();
                    }
                }
                else // current phrase is first phrase
                {
                    // no previous phrase so place play cursor to start of current phrase
                    PlayFromStartOfPhrase();
                }
            } // threshold if ends

        }


        // moves play cursor to starting of current phrase
        private void PlayFromStartOfPhrase()
        {
            if (mPlayer.State == AudioPlayerState.Playing)
            {
                mPausePosition = 0;
                mPlayer.Stop();
                mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]));
            }
            //else
            else if (mPlayListState == AudioPlayerState.Paused)
            {
                mPausePosition = 0;
            }
            else if (mPlayListState == AudioPlayerState.Stopped)
            {
                mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]));
            }
        }

        /// <summary>
        /// Move to the first phrase of the next section.
        /// Stop if there is no next section.
        /// This is only possible when playing the whole book (i.e. the control is grayed out when playing a selection.)
        /// </summary>
        public void NavigateNextSection()
        {
            System.Diagnostics.Debug.Assert(mWholeBook);
        }

        /// <summary>
        /// Move to the first phrase of the previous section, or of this section if we are not yet past the initial threshold.
        /// This is only possible when playing the whole book.
        /// </summary>
        public void NavigatePreviousSection()
        {
            System.Diagnostics.Debug.Assert(mWholeBook);
        }

        private void SetTimeInPlayList(double time)
        {
            if (time >= 0 && time < this.mTotalTime)    // time is within bounds of PlayList
            {
                int PhraseIndex = 0 ;
                double TimeSum = 0 ;

                while (time > TimeSum)
                {
                    TimeSum =  TimeSum + Project.GetAudioMediaAsset ( mPhrases [ PhraseIndex ]).LengthInMilliseconds;
                    PhraseIndex++;
                }
                mCurrentPhraseIndex = PhraseIndex  - 1;
                mElapsedTime =  TimeSum - Project.GetAudioMediaAsset ( mPhrases [ mCurrentPhraseIndex ]).LengthInMilliseconds;
                mPausePosition = time - mElapsedTime;
                
                //MessageBox.Show("Index" + mCurrentPhraseIndex.ToString());
                //MessageBox.Show( "Pause" + mPausePosition.ToString());

                if (mPlayListState == AudioPlayerState.Playing)
                {
                    mPlayer.Stop();    
//                    System.Threading.Thread.Sleep(100);
                    mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]) , mPausePosition );
                }
                else if (mPlayListState == AudioPlayerState.Stopped)
                {
                    
                    mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex] ) , mPausePosition );
                    mPlayListState = AudioPlayerState.Playing;
                }
            }       // End Of  of bound check
            

        }

        private double GetCurrentTime()
        {

            if (mPlayListState == AudioPlayerState.Playing)
            {
                return mElapsedTime + mPlayer.GetCurrentTimePosition();
            }
            else
            {
                return mElapsedTime + mPausePosition;
            }
        }

    }   // end of class
}
