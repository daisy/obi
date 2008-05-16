using Obi.Audio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio ;

namespace Obi
{
    /// <summary>
    /// The playlist is the list of phrases to be played. They are either the ones that were selected by the
    /// user, or just the phrases from the book. The playlist knows how to play itself thanks to the application's
    /// audio player. It implements all the controls of the transport bar except for recording.
    /// </summary>
    public class Playlist
    {
        private AudioPlayer mPlayer;              // audio player for actually playing
        private List<PhraseNode> mPhrases;        // list of phrase nodes in playback order
        private List<double> mStartTimes;         // start time of every phrase
        private int mCurrentPhraseIndex;          // index of the phrase currently playing
        private double mTotalTime;                // total time of this playlist
        private double mElapsedTime;              // elapsed time *before* the beginning of the current asset
        private bool mIsMaster;                   // flag for playing whole book or just a selection
        private AudioPlayerState mPlaylistState;  // playlist state is not always the same as the player state
        private PlayBackState mPlayBackState;     // normal/forward/rewind
        private int mPlaybackRate;                // current playback rate (multiplier)

        private double mPlaybackStartTime;        // start time in first asset
        private double mPlaybackEndTime;          // end time in last asset; negative value if until the end.

        private enum PlayBackState { Normal, Forward, Rewind };
        private static readonly int[] PlaybackRates = { 1, 2, 4, 8 };

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

        // Moved to a new phrase (while playing, or paused), let Obi show it to the user.
        public delegate void MovedToPhraseHandler(object sender, Events.Node.PhraseNodeEventArgs e);
        public event MovedToPhraseHandler MovedToPhrase;

        // Changed the playback rate.
        public delegate void PlaybackRateChangedHandler(object sender, EventArgs e);
        public event PlaybackRateChangedHandler PlaybackRateChanged;

        /// <summary>
        /// Create an empty playlist (to be populated later) for a given player.
        /// </summary>
        public Playlist(AudioPlayer player)
        {
            mPlayer = player;
            Reset(MasterPlaylist);
        }

        /// <summary>
        /// Create a playlist for a player and a selection.
        /// If the selection has a phrase node, add only this node to the playlist.
        /// If the selection has a section node, add all of its phrases to the playlist
        /// (just the strip if selected in the content view; whole section from TOC view.)
        /// If the selection is an audio selection, set start/stop times as well.
        /// </summary>
        public Playlist(AudioPlayer player, NodeSelection selection)
        {
            mPlayer = player;
            Reset(LocalPlaylist);
            if (selection.Control is Obi.ProjectView.TOCView)
            {
                AddPhraseNodes(selection.Node);
            }
            else
            {
                AddPhraseNodesFromStripOrPhrase(selection.Node);
            }
            if (selection is AudioSelection)
            {
                AudioSelection s = (AudioSelection)selection;
                mPlaybackStartTime = s.AudioRange.HasCursor ? s.AudioRange.CursorTime : s.AudioRange.SelectionBeginTime;
                // if a range, should have an end time as well
            }
        }


        /// <summary>
        /// Get the audio player for the playlist. Useful for setting up event listeners.
        /// </summary>
        public AudioPlayer Audioplayer { get { return mPlayer; } }

        public bool CanNavigatePrevPhrase { get { return mCurrentPhraseIndex > 0; } }
        public bool CanNavigateNextPhrase { get { return mCurrentPhraseIndex < mPhrases.Count - 1; } }
        public bool CanNavigateNextSection { get { return NextSectionIndex < mPhrases.Count - 1; } }
        public bool CanNavigateNextPage { get { return true; } }

        public PhraseNode NextPage(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node) + 1;
            for (; index < mPhrases.Count && mPhrases[index].NodeKind != EmptyNode.Kind.Page; ++index) { }
            return index >= 0 && index < mPhrases.Count ? mPhrases[index] : null;
        }

        public PhraseNode NextPhrase(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node) + 1;
            return index < mPhrases.Count ? mPhrases[index] : null;
        }

        public PhraseNode NextSection(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node);
            if (node != null)
            {
                for (; index < mPhrases.Count && mPhrases[index].AncestorAs<SectionNode>() == node.AncestorAs<SectionNode>(); ++index) { }
            }
            return index >= 0 && index < mPhrases.Count ? mPhrases[index] : null;
        }

        public PhraseNode PrevPage(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node) - 1;
            for (; index >= 0 && mPhrases[index].NodeKind != EmptyNode.Kind.Page; --index) { }
            return index >= 0 ? mPhrases[index] : null;
        }

        public PhraseNode PrevPhrase(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node) - 1;
            return index >= 0 ? mPhrases[index] : null;
        }


        public PhraseNode PrevSection(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node);
            if (node != null)
            {
                for (; index >= 0 && mPhrases[index].AncestorAs<SectionNode>() == node.AncestorAs<SectionNode>(); --index) { }
                if (index >= 0)
                {
                    SectionNode prev = mPhrases[index].AncestorAs<SectionNode>();
                    for (; index >= 0 && mPhrases[index].AncestorAs<SectionNode>() == prev; --index) { }
                    ++index;
                }
            }
            return index >= 0 ? mPhrases[index] : null;
        }

        /// <summary>
        /// Set the currently playing phrase directly.
        /// If playing, move to the beginning of the phrase.
        /// If the phrase is not in the playlist, stay on the same phrase.
        /// </summary>
        public PhraseNode CurrentPhrase
        {
            get { return mPhrases.Count > 0 ? mPhrases[mCurrentPhraseIndex] : null; }
            set
            {
                bool playing = mPlaylistState == AudioPlayerState.Playing;
                if (playing) Stop();
                int index = mPhrases.IndexOf(value);
                if (index >= 0)
                {
                    mCurrentPhraseIndex = index;
                    mElapsedTime = mStartTimes[mCurrentPhraseIndex];
                }
                if (playing) Play();
            }
        }

        /// <summary>
        /// The section in which the currently playing phrase is.
        /// </summary>
        public SectionNode CurrentSection
        {
            get { return mPhrases.Count > 0 ? mPhrases[mCurrentPhraseIndex].ParentAs<SectionNode>() : null; }
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
            get { return mPlayer.CurrentTimePosition; }
            set
            {
                if (value >= 0 &&
                    value < mPhrases[mCurrentPhraseIndex].Audio.getDuration().getTimeDeltaAsMillisecondFloat())
                {
                    mPlayer.CurrentTimePosition = value;
                }
            }
        }

        /// <summary>
        /// First phrase in the playlist, or null if empty.
        /// </summary>
        public PhraseNode FirstPhrase { get { return mPhrases.Count > 0 ? mPhrases[0] : null; } }

        /// <summary>
        /// Play from the current phrase.
        /// </summary>
        public void Play()
        {
            System.Diagnostics.Debug.Assert(mPlaylistState == AudioPlayerState.Stopped, "Only play from stopped state.");
            if (mCurrentPhraseIndex < mPhrases.Count) PlayPhrase(mCurrentPhraseIndex);
        }

        public void Play(double from)
        {
            mPlaybackStartTime = from;
            Play();
        }

        public void Play(double from, double to)
        {
            mPlaybackEndTime = to;
            Play(from);
        }

        /// <summary>
        /// Set a new presentation for this playlist; i.e., regenerate the master playlist for the presentation.
        /// </summary>
        public Presentation Presentation
        {
            set
            {
                Reset(MasterPlaylist);
                if (value != null) AddPhraseNodes(value.RootNode);
                value.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                value.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
            }
        }

        /// <summary>
        /// The state of the playlist, as opposed to that of the underlying player.
        /// </summary>
        public AudioPlayerState State { get { return mPlaylistState; } }


        // Add all phrase nodes underneath (and including) the starting node.
        // In case of the master playlist, exclude unused nodes.
        private void AddPhraseNodes(urakawa.core.TreeNode node)
        {
            node.acceptDepthFirst
            (
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is PhraseNode && n.getChildCount() == 0 && (!mIsMaster || ((PhraseNode)n).Used))
                    {
                        mPhrases.Add((PhraseNode)n);
                        mStartTimes.Add(mTotalTime);
                        mTotalTime += ((PhraseNode)n).Audio.getDuration().getTimeDeltaAsMillisecondFloat();
                    }
                    return true;
                },
                delegate(urakawa.core.TreeNode n) { }
            );
        }

        // Add phrase nodes from a strip, or a single phrase.
        // Shallow operation compared to AddPhraseNode which is deep.
        private void AddPhraseNodesFromStripOrPhrase(ObiNode node)
        {
            if (node is PhraseNode)
            {
                mPhrases.Add((PhraseNode)node);
                mStartTimes.Add(mTotalTime);
                mTotalTime += ((PhraseNode)node).Audio.getDuration().getTimeDeltaAsMillisecondFloat();
            }
            else if (node is SectionNode)
            {
                for (int i = 0; i < ((SectionNode)node).PhraseChildCount; ++i)
                {
                    AddPhraseNodesFromStripOrPhrase(((SectionNode)node).PhraseChild(i));
                }
            }
        }

        // Insert new tree nodes in the right place in the playlist.
        private void InsertNode(urakawa.core.TreeNode node)
        {
            if (!(node is ObiNode) || !((ObiNode)node).IsRooted) return;
            // Find where new nodes would have to be added
            ObiNode prev = ((ObiNode)node).PrecedingNode;
            while (prev != null && !(prev is PhraseNode && mPhrases.Contains((PhraseNode)prev))) prev = prev.PrecedingNode;
            int index = prev == null ? 0 : (mPhrases.IndexOf((PhraseNode)prev) + 1);
            // Add all of the used phrase nodes that we could find
            node.acceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is PhraseNode && ((PhraseNode)n).Used)
                    {
                        double time = ((PhraseNode)n).Audio.getDuration().getTimeDeltaAsMillisecondFloat();
                        mPhrases.Insert(index, (PhraseNode)n);
                        mStartTimes.Add(0.0);
                        mStartTimes[index] = index == 0 ? 0.0 :
                            (mStartTimes[index - 1] + mPhrases[index - 1].Audio.getDuration().getTimeDeltaAsMillisecondFloat());
                        mTotalTime += time;
                        ++index;
                    }
                    return true;
                }, delegate(urakawa.core.TreeNode n) { }
            );
            for (int i = index; i < mStartTimes.Count - 1; ++i)
            {
                mStartTimes[i + 1] = mStartTimes[i] + mPhrases[i].Audio.getDuration().getTimeDeltaAsMillisecondFloat();
            }
        }

        // Play the current phrase
        private void PlayCurrentPhrase()
        {
            Events.Audio.Player.StateChangedEventArgs evargs = new Events.Audio.Player.StateChangedEventArgs(mPlayer.State);
            if (mPlaylistState == AudioPlayerState.Stopped)
            {
                mPlayer.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(Playlist_MoveToNextPhrase);
            }
            mPlaylistState = AudioPlayerState.Playing;
            double from = mPlaybackStartTime;
            mPlaybackStartTime = 0.0;
            if (mCurrentPhraseIndex == mPhrases.Count - 1 && mPlaybackEndTime > 0.0)
            {
                mPlayer.Play(mPhrases[mCurrentPhraseIndex].Audio.getMediaData(), from, mPlaybackEndTime);
            }
            else
            {
                mPlayer.Play(mPhrases[mCurrentPhraseIndex].Audio.getMediaData(), from);
            }
            // send the state change event if the state actually changed
            if (StateChanged != null && mPlayer.State != evargs.OldState) StateChanged(this, evargs);
        }

        // Play the phrase at the given index in the list.
        private void PlayPhrase(int index)
        {
            SkipToPhrase(index);
            PlayCurrentPhrase();
        }

        // React to addition and removal of tree nodes in the presentation.
        private void Presentation_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (e is urakawa.events.core.ChildAddedEventArgs)
            {
                InsertNode(((urakawa.events.core.ChildAddedEventArgs)e).AddedChild);
            }
            else if (e is urakawa.events.core.ChildRemovedEventArgs)
            {
                RemoveNode(((urakawa.events.core.ChildRemovedEventArgs)e).RemovedChild);
            }
        }

        // React to nodes being marked used or unused by adding or removing them from the playlist.
        // What about section nodes?!
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (e.Node is PhraseNode)
            {
                if (e.Node.Used)
                {
                    InsertNode(e.Node);
                }
                else
                {
                    RemoveNode(e.Node);
                }
            }
        }

        // Remove a node and all of its contents from the playlist
        private void RemoveNode(urakawa.core.TreeNode node)
        {
            int updateTimeFrom = mPhrases.Count;
            node.acceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is PhraseNode && mPhrases.Contains((PhraseNode)n))
                    {
                        int index = mPhrases.IndexOf((PhraseNode)n);
                        if (updateTimeFrom == mPhrases.Count) updateTimeFrom = index == 0 ? 1 : index;
                        mPhrases.RemoveAt(index);
                        if (index < mStartTimes.Count - 1) mStartTimes.RemoveAt(index + 1);
                        mTotalTime -= ((PhraseNode)n).Audio.getDuration().getTimeDeltaAsMillisecondFloat();
                    }
                    return true;
                },
                delegate(urakawa.core.TreeNode n) { }
            );
            for (int i = updateTimeFrom; i < mPhrases.Count; ++i)
            {
                mStartTimes[i] = mStartTimes[i - 1] + mPhrases[i - 1].Audio.getDuration().getTimeDeltaAsMillisecondFloat();
            }
        }

        private static readonly bool MasterPlaylist = true;  // value for Reset
        private static readonly bool LocalPlaylist = false;  // value for Reset

        // Reset the playlist.
        private void Reset(bool isMaster)
        {
            mPhrases = new List<PhraseNode>();
            mStartTimes = new List<double>();
            mTotalTime = 0.0;
            mPlaybackRate = 0;
            mPlayBackState = PlayBackState.Normal;
            mPlaylistState = mPlayer.State;
            mIsMaster = isMaster;
            mPlaybackStartTime = 0.0;
            mPlaybackEndTime = -1.0;
        }

        // Skip to the beginning of a phrase at a given index, provided that it is in the playlist range.
        private void SkipToPhrase(int index)
        {
            System.Diagnostics.Debug.Assert(index >= 0 && index < mPhrases.Count, "Phrase index out of range!");
            mCurrentPhraseIndex = index;
            mElapsedTime = mStartTimes[mCurrentPhraseIndex];
            int mode = mPlayer.PlaybackFwdRwdRate;
            if (MovedToPhrase != null) MovedToPhrase(this, new Events.Node.PhraseNodeEventArgs(this, mPhrases[mCurrentPhraseIndex]));
            mPlayer.Stop();
            mPlayer.PlaybackFwdRwdRate = mode;
        }


























        


        /// <summary>
        /// Index of the first phrase of the next section, or number of phrases if there is no next section.
        /// </summary>
        public int NextSectionIndex
        {
            get
            {
                int i = mCurrentPhraseIndex + 1;
                for (; i < mPhrases.Count && mPhrases[i].ParentAs<SectionNode>() == CurrentSection; ++i) { }
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
                for (; first >= 0 && mPhrases[first].ParentAs<SectionNode>() == CurrentSection; --first) { }
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
                    SectionNode previousSection = mPhrases[previous].ParentAs<SectionNode>();
                    // go back while we are in the previous section
                    for (; previous >= 0 && mPhrases[previous].ParentAs<SectionNode>() == previousSection; --previous) { }
                    // we went back one too many
                    return previous + 1;
                }
            }
        }

        /// <summary>
        /// Get the total time for this playlist in milliseconds.
        /// </summary>
        public double TotalTime { get { return mTotalTime; } }

        /// <summary>
        /// Get the total time for the current asset in milliseconds. 
        /// </summary>
        public double TotalAssetTime
        {
            get
            {
                return mCurrentPhraseIndex >= 0 && mCurrentPhraseIndex < mPhrases.Count ?
                    mPhrases[mCurrentPhraseIndex].Audio.getDuration().getTimeDeltaAsMillisecondFloat() : 0.0;
            }
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
                return mPhrases[mCurrentPhraseIndex].Audio.getDuration().getTimeDeltaAsMillisecondFloat() -
                    (mPlayer.CurrentTimePosition);
            }
            set
            {
                CurrentTimeInAsset = mPhrases[mCurrentPhraseIndex].Audio.getDuration().getTimeDeltaAsMillisecondFloat() - value;
            }
        }

        /// <summary>
        /// Playing the whole book or just a selection.
        /// </summary>
        public bool WholeBook { get { return mIsMaster; } }

        public int PlaybackRate
        {
            get { return PlaybackRates[mPlaybackRate] * (mPlayBackState == PlayBackState.Rewind ? -1 : 1); }
        }





        
        

        /// <summary>
        /// Resume playing from current point.
        /// </summary>
        public void Resume()
        {
            System.Diagnostics.Debug.Assert(mPlaylistState == AudioPlayerState.Paused, "Only resume from paused state.");
            mPlaylistState = AudioPlayerState.Playing;
            mPlayer.Resume();
            // TODO: mPlayer.Play(mPhrases[mCurrentPhraseIndex].Asset, mPausePosition);
            if (StateChanged != null)
            {
                StateChanged(this, new Events.Audio.Player.StateChangedEventArgs(AudioPlayerState.Paused));
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


            if ( //mPlayer.PlaybackMode == Audio.PlaybackMode.Rewind
                mPlayer.PlaybackFwdRwdRate < 0
                && mCurrentPhraseIndex > 0)
                PlayPhrase(mCurrentPhraseIndex - 1);
            else if (mCurrentPhraseIndex < mPhrases.Count - 1 && //mPlayer.PlaybackMode != Audio.PlaybackMode.Rewind
                mPlayer.PlaybackFwdRwdRate >= 0)
            {
                PlayPhrase(mCurrentPhraseIndex + 1);
            }
            else if (EndOfPlaylist != null)
            {
                //mPlaylistState = AudioPlayerState.Stopped;    // Avn: Commented because changing Playlist state to stopped before calling stop () function will bypass stopping code
                Stop();
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
                mPlaylistState = AudioPlayerState.Paused;
                mPlayer.Pause();
                //mPlayer.PlaybackMode = PlaybackMode.Normal;
                mPlayer.PlaybackFwdRwdRate = 0;
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
            if (mPlaylistState == AudioPlayerState.Playing || mPlaylistState == AudioPlayerState.Paused)
            {
                Events.Audio.Player.StateChangedEventArgs evargs = new Events.Audio.Player.StateChangedEventArgs(mPlayer.State);
                mPlayer.Stop();
                //mPlayer.PlaybackMode = PlaybackMode.Normal;
                mPlayer.PlaybackFwdRwdRate = 0;
                //System.Media.SystemSounds.Asterisk.Play();
                mPlaylistState = AudioPlayerState.Stopped;
                if (StateChanged != null) StateChanged(this, evargs);

                mCurrentPhraseIndex = 0;
                mElapsedTime = 0.0;
                System.Diagnostics.Debug.Print("--- end of audio asset handler unset");
                mPlayer.EndOfAudioAsset -= new Events.Audio.Player.EndOfAudioAssetHandler(Playlist_MoveToNextPhrase);
            }
        }

        /// <summary>
        /// Start or resume playing backward at a faster rate.
        /// </summary>
        public void Rewind()
        {
            //if (mPlayer.PlaybackMode != Audio.PlaybackMode.Rewind)
            if (mPlayer.PlaybackFwdRwdRate >= 0)
            {
                mPlaybackRate = 1;
                mPlayer.PlaybackFwdRwdRate = mPlaybackRate * -1;
                //mPlayer.PlaybackMode = Audio.PlaybackMode.Rewind;

                if (mPlayer.State == AudioPlayerState.Paused)
                    mPlayer.Resume();
                else if (mPlayer.State == AudioPlayerState.Stopped)
                    Play();

                mPlayBackState = PlayBackState.Rewind;
            }
            else
            {
                IncreasePlaybackRate();
                mPlayer.PlaybackFwdRwdRate = mPlaybackRate * -1;
            }
            if (PlaybackRateChanged != null)
                PlaybackRateChanged(this, new EventArgs());
        }


        public void FastForward()
        {
            //if (mPlayer.PlaybackMode !=Audio.PlaybackMode.FastForward  )
            if (mPlayer.PlaybackFwdRwdRate <= 0)
            {
                mPlaybackRate = 1;
                mPlayer.PlaybackFwdRwdRate = mPlaybackRate;
                //mPlayer.PlaybackMode = Audio.PlaybackMode.FastForward;

                if (mPlayer.State == AudioPlayerState.Paused)
                    mPlayer.Resume();
                else if (mPlayer.State == AudioPlayerState.Stopped)
                    Play();

                mPlayBackState = PlayBackState.Forward;
            }
            else
            {
                IncreasePlaybackRate();
                mPlayer.PlaybackFwdRwdRate = mPlaybackRate;
            }
            if (PlaybackRateChanged != null)
                PlaybackRateChanged(this, new EventArgs());
        }

        /// <summary>
        /// Increase the playback rate, if we're at the max go back to the first notch above 1.
        /// </summary>
        private void IncreasePlaybackRate()
        {
            ++mPlaybackRate;
            if (mPlaybackRate == PlaybackRates.Length)
                mPlaybackRate = 1;

            mPlayer.PlaybackFwdRwdRate = mPlaybackRate;
        }


        /// <summary>
        /// Navigate to previous page in playlist, do nothing if current phrase is first page node of playlist
                /// </summary>
        public void NavigateToPreviousPage ()
        {
            if (mCurrentPhraseIndex > 0)
            {
                int PrevPagePhraseIndex = mCurrentPhraseIndex - 1;
                PhraseNode n = mPhrases[PrevPagePhraseIndex];

                while (PrevPagePhraseIndex > 0
                    && n.NodeKind != EmptyNode.Kind.Page)
                {
                    --PrevPagePhraseIndex;
                    n = (PhraseNode)mPhrases[PrevPagePhraseIndex];
                }

                if (PrevPagePhraseIndex < mCurrentPhraseIndex && PrevPagePhraseIndex >= 0
                    &&     n.NodeKind == EmptyNode.Kind.Page )
                                    NavigateToPhrase(PrevPagePhraseIndex);
                            }
        }


        /// <summary>
        /// Move to the first phrase of the previous section, or of this section if we are not yet past the initial threshold.
        /// </summary>
        public void NavigateToPreviousSection()
        {
            if ( mPhrases.Count > 0 ) NavigateToPhrase(PreviousSectionIndex);
        }

        /// <summary>
        /// Move back one phrase.
        /// If the current position is past the initial threshold, move back to the beginning of the current phrase.
        /// When there is no previous phrase, move to the beginning of the current phrase.
        /// </summary>
        public void NavigateToPreviousPhrase()
        {
            if (mPhrases.Count > 0)
            {
                double currentTime = mPlayer.State == AudioPlayerState.Playing ? mPlayer.CurrentTimePosition : 0.0;
                NavigateToPhrase(mCurrentPhraseIndex -
                    (currentTime > InitialThreshold || mCurrentPhraseIndex == 0 ? 0 : 1));
            }
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
            if ( mPhrases.Count > 0      &&    next != mCurrentPhraseIndex && next < mPhrases.Count) NavigateToPhrase(NextSectionIndex);
        }

        /// <summary>
        /// Navigate to next page in playlist, do nothing if current phrase is last page
                /// </summary>
        public void NavigateToNextPage()
        {
            if (mCurrentPhraseIndex < mPhrases.Count - 1)
            {
                int NextPagePhraseIndex = mCurrentPhraseIndex + 1;
                PhraseNode n = mPhrases[NextPagePhraseIndex];

                while (NextPagePhraseIndex < mPhrases.Count - 1
                    && n.NodeKind != EmptyNode.Kind.Page)
                {
                    ++NextPagePhraseIndex;
                    n = (PhraseNode)mPhrases[NextPagePhraseIndex];
                }

                if ( NextPagePhraseIndex > mCurrentPhraseIndex && NextPagePhraseIndex < mPhrases.Count
                    &&     n.NodeKind == EmptyNode.Kind.Page )
                    NavigateToPhrase(NextPagePhraseIndex);
            }
        }


        /// <summary>
        /// Navigate to a phrase and pause, keep playing or start playing depending on the state.
        /// If the index is the same as the current, the current phrase will restart, so don't call this
        /// if you don't want this behavior.
        /// </summary>
        /// <param name="index">The index of the phrase to navigate to.</param>
        private void NavigateToPhrase(int index)
        {
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
        /// Add a new phrase node at the right spot in the (master) playlist.
        /// The phrase that comes before it should already be in the playlist.
        /// </summary>
        /// <param name="node">The phrase node to add.</param>
        public void AddPhrase(PhraseNode node)
        {
            PhraseNode prev = node.PrecedingPhraseInProject;
            int index = prev == null ? 0 : mPhrases.IndexOf(prev) + 1;
            mPhrases.Insert(index, node);
            mStartTimes.Add(0.0);
            if (index > 1) UpdateTimeFromIndex(index - 1);
        }

        /// <summary>
        /// Remove a phrase node from the (master) playlist.
        /// </summary>
        /// <param name="node">The phrase node to remove.</param>
        public void RemovePhrase(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node);
            mPhrases.RemoveAt(index);
            mStartTimes.RemoveAt(index);
            if (mPhrases.Count > 0) UpdateTimeFromIndex(index);
        }

        /// <summary>
        /// A node's media has changed so change the timing info.
        /// </summary>
        /// <param name="node"></param>
        public void UpdateTimeFrom(PhraseNode node)
        {
            if (mPhrases.Contains(node))
            {
                int index = mPhrases.IndexOf(node);
                UpdateTimeFromIndex(index);
            }
        }

        private void UpdateTimeFromIndex(int index)
        {
            for (int i = index + 1; i < mStartTimes.Count; ++i)
            {
                mStartTimes[i] = mStartTimes[i - 1] + mPhrases[i - 1].Audio.getDuration().getTimeDeltaAsMillisecondFloat();
            }
            mTotalTime = mStartTimes[mStartTimes.Count - 1] + mPhrases[mStartTimes.Count - 1].Audio.getDuration().getTimeDeltaAsMillisecondFloat();
            System.Diagnostics.Debug.Print("!!! Playlist: {0} phrase(s), length = {1}ms.", mPhrases.Count, mTotalTime);
        }

        public bool ContainsPhrase(PhraseNode phrase)
        {
            return phrase != null && mPhrases.Contains(phrase);
        }

        public void FastPlayNormaliseWithLapseBack(double LapseBackTime)
        {
            if (mPlayer.CurrentTimePosition > LapseBackTime)
            {
                mPlayer.Pause();
                mPlayer.FastPlayFactor = 1;
                mPlayer.CurrentTimePosition = mPlayer.CurrentTimePosition - LapseBackTime;
                mPlayer.Resume();
            }
            else
            {
                mPlayer.Pause();
                mPlayer.CurrentTimePosition = 10;
                mPlayer.FastPlayFactor = 1;
                mPlayer.Resume();
            }
        }




        public void PreviewSelectedFragment(double StartTime , double EndTime )
        {
            AudioMediaData AudioData = mPhrases[mCurrentPhraseIndex].Audio.getMediaData();

            if (StartTime < 0)
                StartTime = 0;

            if ( EndTime >  AudioData.getAudioDuration ().getTimeDeltaAsMillisecondFloat () )
                EndTime = AudioData.getAudioDuration ().getTimeDeltaAsMillisecondFloat ()  ;

            
            mPlayer.PlayPreview(AudioData, StartTime, EndTime, StartTime );

        }
    }
}