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
        private Project mProject;                 // project in which we are playing
        private AudioPlayer mPlayer;              // audio player for actually playing
        private List<CoreNode> mPhrases;          // list of phrase nodes (from which we get the assets)
        private List<double> mStartTimes;         // start time of every phrase
        private int mCurrentPhraseIndex;          // index of the phrase currently playing
        private double mTotalTime;                // total time of this playlist
        private double mElapsedTime;              // elapsed time *before* the beginning of the current asset
        private bool mWholeBook;                  // flag for playing whole book or just a selection
        private double mPausePosition;            // position in the asset where we  paused
        private AudioPlayerState mPlaylistState;  // playlist state is not always the same as the player state

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
        /// The state of the playlist, as opposed to that of the underlying player.
        /// </summary>
        public AudioPlayerState State
        {
            get { return mPlaylistState; }
        }

        /// <summary>
        /// Set the currently playing node directly.
        /// </summary>
        public CoreNode CurrentPhrase
        {
            get { return mPhrases.Count > 0 ? mPhrases[mCurrentPhraseIndex] : null; }
            set
            {
                int i;
                for (i = 0; i < mPhrases.Count && mPhrases[i] != value; ++i) { }
                System.Diagnostics.Debug.Print("Current selection is at index {0}/{1}", i, mPhrases.Count);
                bool playing = mPlaylistState == AudioPlayerState.Playing;
                if (playing) Stop();
                if (i < mPhrases.Count)
                {
                    mCurrentPhraseIndex = i;
                    mPausePosition = 0.0;
                    mElapsedTime = mStartTimes[mCurrentPhraseIndex];
                }
                if (playing) Play();
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
        /// Index of the first phrase of the next section, or number of phrases if there is no next section.
        /// </summary>
        public int NextSectionIndex
        {
            get
            {
                int i = mCurrentPhraseIndex + 1;
                for (; i < mPhrases.Count && mPhrases[i].getParent() == CurrentSection; ++i) { }
                return i;
            }
        }

        /// <summary>
        /// Index of the first phrase of the previous section, or of the first phrase of the current section if we are
        /// past the initial threshold.
        /// </summary>
        private int PreviousSectionIndex
        {
            get
            {
                // find the first phrase of the current section
                int first = mCurrentPhraseIndex;
                for (; first >= 0 && mPhrases[first].getParent() == CurrentSection; --first) { }
                ++first;
                if ((first == 0) || (CurrentTime - mStartTimes[first] > InitialThreshold))
                {
                    // no previous section, or past the initial threshold so just return the first phrase of the current section.
                    return first;
                }
                else
                {
                    // find the first of the previous section 
                    int previous = first - 1;
                    CoreNode previousSection = (CoreNode)mPhrases[previous].getParent();
                    for (; previous >= 0 && mPhrases[previous].getParent() == previousSection; --previous) { }
                    return previous + 1;
                }
            }
        }

        /// <summary>
        /// Get/set the current playing time inside the playlist in milliseconds.
        /// Setting the current time starts/continues playing from that position, or pauses at that position.
        /// </summary>
        public double CurrentTime
        {
            get { return mElapsedTime + CurrentTimeInAsset; }
            set
            {
                if (value >= 0 && value < mTotalTime)
                {
                    int i;
                    for (i = 0; i < mPhrases.Count && mStartTimes[i] <= value; ++i) { }
                    if (i > 0) --i;
                    NavigateToPhrase(i);
                    CurrentTimeInAsset = value - mStartTimes[i];
                }
            }
        }

        /// <summary>
        /// Elapsed time in the current asset in milliseconds.
        /// </summary>
        public double CurrentTimeInAsset
        {
            get { return mPlaylistState == AudioPlayerState.Playing ? mPlayer.GetCurrentTimePosition() : mPausePosition; }
            set
            {
                if (value >= 0 && value < Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]).LengthInMilliseconds)
                {
                    if (mPlaylistState == AudioPlayerState.Playing)
                    {
                        mPlayer.CurrentTimePosition = value;
                    }
                    else
                    {
                        mPausePosition = value;
                    }
                }
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
            set { CurrentTime = mTotalTime - value; }
        }

        /// <summary>
        /// Remaining time in the currently playing asset in milliseconds.
        /// </summary>
        public double RemainingTimeInAsset
        {
            get
            {
                return Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]).LengthInMilliseconds -
                    (mPlaylistState == AudioPlayerState.Playing ? mPlayer.GetCurrentTimePosition() : mPausePosition);
            }
            set
            {
                CurrentTimeInAsset = Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]).LengthInMilliseconds - value;
            }
        }

        /// <summary>
        /// Playing the whole book or just a selection.
        /// </summary>
        public bool WholeBook
        {
            get { return mWholeBook; }
        }

        /// <summary>
        /// Create a playlist for the whole project.
        /// The playlist contains all phrase nodes.
        /// </summary>
        /// <param name="project">The project for this playlist.</param>
        /// <param name="player">The audio player for this playlist.</param>
        public Playlist(Project project, AudioPlayer player)
        {
            InitPlaylist(project, player, project.RootNode, true);
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
            mStartTimes = new List<double>();
            mCurrentPhraseIndex = 0;
            mElapsedTime = 0.0;
            mTotalTime = 0.0;
            mWholeBook = wholeBook;
            mPausePosition = 0;
            mPlaylistState = mPlayer.State;
            System.Diagnostics.Debug.Assert(mPlaylistState == AudioPlayerState.Stopped,
                "Audio player and playlist should be stopped.");
            node.visitDepthFirst
            (
                // Add all phrase nodes underneath (and including) the starting node.
                delegate(ICoreNode n)
                {
                    if (Project.GetNodeType((CoreNode)n) == NodeType.Phrase)
                    {
                        mPhrases.Add((CoreNode)n);
                        mStartTimes.Add(mTotalTime);
                        mTotalTime += Project.GetAudioMediaAsset((CoreNode)n).LengthInMilliseconds;
                    }
                    return true;
                },
                // nothing to do in post-visit
                delegate(ICoreNode n) { }
            );
            // mPlayer.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(Playlist_MoveToNextPhrase);
            mPlayer.StateChanged += new Events.Audio.Player.StateChangedHandler
            (
                // Intercept state change events from the player, and only pass along those that
                // involve the "not ready" state.
                delegate(object sender, Events.Audio.Player.StateChangedEventArgs e)
                {
                    if (e.OldState == AudioPlayerState.NotReady || mPlayer.State == AudioPlayerState.NotReady)
                    {
                        StateChanged(this, e);
                    }
                }
            );
        }

        /// <summary>
        /// Play.
        /// </summary>
        public void Play()
        {
            if (mPhrases.Count > 0)
            {
                if (mPlaylistState == AudioPlayerState.Paused)
                {
                    // Resume by using play from function
                    mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]), mPausePosition);
                    mPlaylistState = AudioPlayerState.Playing;
                    if (StateChanged != null)
                    {
                        StateChanged(this, new Events.Audio.Player.StateChangedEventArgs(AudioPlayerState.Paused));
                    }
                    if (MovedToPhrase != null)
                    {
                        MovedToPhrase(this, new Events.Node.NodeEventArgs(this, mPhrases[mCurrentPhraseIndex]));
                    }
                }
                else if (mPlaylistState == AudioPlayerState.Stopped)
                {
                    mPlayer.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(Playlist_MoveToNextPhrase);
                    mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]));
                    mPlaylistState = AudioPlayerState.Playing;
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
        private void Playlist_MoveToNextPhrase(object sender, Events.Audio.Player.EndOfAudioAssetEventArgs e)
        {
            // add an option to have a beep between assets
            // System.Media.SystemSounds.Exclamation.Play();
            if (mCurrentPhraseIndex < mPhrases.Count - 1)
            {
                PlayPhrase(mCurrentPhraseIndex + 1);
            }
            else if (EndOfPlaylist != null)
            {
                mPlaylistState = AudioPlayerState.Stopped;
                EndOfPlaylist(this, new EventArgs());
            }
        }

        /// <summary>
        /// Pause.
        /// </summary>
        public void Pause()
        {
            if (mPlaylistState == AudioPlayerState.Playing)
            {
                mPausePosition = mPlayer.CurrentTimePosition;
                mPlayer.Stop();
                mPlaylistState = AudioPlayerState.Paused;
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
            if (mPlaylistState != AudioPlayerState.Stopped)
            {
                mElapsedTime = 0.0;
                Events.Audio.Player.StateChangedEventArgs evargs = new Events.Audio.Player.StateChangedEventArgs(mPlayer.State);
                mPlayer.Stop();
                mPlaylistState = AudioPlayerState.Stopped;
                if (StateChanged != null) StateChanged(this, evargs);
                mPlayer.EndOfAudioAsset -= new Events.Audio.Player.EndOfAudioAssetHandler(Playlist_MoveToNextPhrase);
            }
        }

        /// <summary>
        /// Move to the first phrase of the previous section, or of this section if we are not yet past the initial threshold.
        /// </summary>
        public void NavigateToPreviousSection()
        {
            NavigateToPhrase(PreviousSectionIndex);
        }

        /// <summary>
        /// Move back one phrase.
        /// If the current position is past the initial threshold, move back to the beginning of the current phrase.
        /// When there is no previous phrase, move to the beginning of the current phrase.
        /// </summary>
        public void NavigateToPreviousPhrase()
        {
            NavigateToPhrase(mCurrentPhraseIndex -
                (mPlayer.CurrentTimePosition > InitialThreshold || mCurrentPhraseIndex == 0 ? 0 : 1));
        }

        /// <summary>
        /// Move to the next phrase. Do nothing if we are already at the last phrase.
        /// </summary>
        public void NavigateToNextPhrase()
        {
            if (mCurrentPhraseIndex < mPhrases.Count - 1) NavigateToPhrase(mCurrentPhraseIndex + 1);
        }

        /// <summary>
        /// Move to the first phrase of the next section. Do nothing if we are already in the last section.
        /// </summary>
        public void NavigateToNextSection()
        {
            int next = NextSectionIndex;
            if (next != mCurrentPhraseIndex && next < mPhrases.Count) NavigateToPhrase(NextSectionIndex);
        }

        /// <summary>
        /// Navigate to a phrase and pause, keep playing or start playing depending on the state.
        /// If the index is the same as the current, the current phrase will restart, so don't call this
        /// if you don't want this behavior.
        /// </summary>
        /// <param name="index">The index of the phrase to navigate to.</param>
        private void NavigateToPhrase(int index)
        {
            System.Diagnostics.Debug.Assert(mPlayer.State != AudioPlayerState.NotReady, "Player is not ready!");
            if (mPlaylistState == AudioPlayerState.Playing)
            {
                mPlayer.Stop();
                PlayPhrase(index);
            }
            else if (mPlaylistState == AudioPlayerState.Paused)
            {
                SkipToPhrase(index);
            }
            else if (mPlaylistState == AudioPlayerState.Stopped)
            {
                PlayPhrase(index);
            }
        }

        /// <summary>
        /// Play the phrase at some index in the list.
        /// </summary>
        /// <param name="index">The index of the phrase to play.</param>
        private void PlayPhrase(int index)
        {
            SkipToPhrase(index);
            PlayCurrentPhrase();
        }

        private void PlayCurrentPhrase()
        {
            System.Diagnostics.Debug.Print("Play current phrase #{0}", mCurrentPhraseIndex);
            Events.Audio.Player.StateChangedEventArgs evargs = new Events.Audio.Player.StateChangedEventArgs(mPlayer.State);
            mPlayer.Play(Project.GetAudioMediaAsset(mPhrases[mCurrentPhraseIndex]));
            mPlaylistState = AudioPlayerState.Playing;
            // send the state change event if the state actually changed
            if (StateChanged != null && mPlayer.State != evargs.OldState) StateChanged(this, evargs);
        }

        /// <summary>
        /// Skip to a phrase at a given index.
        /// </summary>
        /// <param name="index">Index of the phrase to skip to.</param>
        private void SkipToPhrase(int index)
        {
            System.Diagnostics.Debug.Assert(index >= 0 && index < mPhrases.Count, "Phrase index out of range!");
            mCurrentPhraseIndex = index;
            mPausePosition = 0.0;
            mElapsedTime = mStartTimes[mCurrentPhraseIndex];
            if (MovedToPhrase != null) MovedToPhrase(this, new Events.Node.NodeEventArgs(this, mPhrases[mCurrentPhraseIndex]));
        }
    }
}