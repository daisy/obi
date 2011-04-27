using System.IO;
using AudioLib;
using Obi.Audio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using urakawa;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio ;
using urakawa.media.timing;
using urakawa.property;

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
        protected List<PhraseNode> mPhrases;        // list of phrase nodes in playback order
        private List<double> mStartTimes;         // start time of every phrase
        protected int mCurrentPhraseIndex;          // index of the phrase currently playing
        private double mTotalTime;                // total time of this playlist
        private double mElapsedTime;              // elapsed time *before* the beginning of the current asset
        private bool mIsMaster;                   // flag for playing whole book or just a selection
        private AudioPlayer.State mPlaylistState;  // playlist state is not always the same as the player state
        private PlayBackState mPlayBackState;     // normal/forward/rewind
        private int mPlaybackRate;                // current playback rate (multiplier)

        private double mPlaybackStartTime;        // start time in first asset
        private double mPlaybackEndTime;          // end time in last asset; negative value if until the end.
        private bool mIsQAPlaylist; // flag to indicate that master playlist is QA playlist i.e. do not contain unused phrases

        private enum PlayBackState { Normal, Forward, Rewind };
        private static readonly int[] PlaybackRates = { 1, 2, 4, 8 };

        // Amount of time after which "previous phrase" goes to the beginning of the phrase
        // rather than the actual previous phrase. In milliseconds.
        private static readonly double InitialThreshold = 1500.0;

        // The playlist sends its own version of the state changed event in order to ignore spurrious
        // stop/start events sent by the audio player when moving between assets.
        // The VUmeter event should be caught as is; the end of asset should be ignored.
        public event AudioPlayer.StateChangedHandler StateChanged;

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
        public Playlist(AudioPlayer player  , bool IsQAPlaylist )
        {
            resetEndOfAudioTimer();

            mPlayer = player;
            mIsQAPlaylist = IsQAPlaylist;
            Reset(MasterPlaylist );
        }

        /// <summary>
        /// Create a playlist for a player and a selection.
        /// If the selection has a phrase node, add only this node to the playlist.
        /// If the selection has a section node, add all of its phrases to the playlist
        /// (just the strip if selected in the content view; whole section from TOC view.)
        /// If the selection is an audio selection, set start/stop times as well.
        /// </summary>
        public Playlist(AudioPlayer player, NodeSelection selection , bool IsQAPlaylist )
        {
            resetEndOfAudioTimer();

            mIsQAPlaylist = IsQAPlaylist;
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
        /// If phrase node is passed as parameter, adds this phrase in playlist
        ///  else if section node is passed, add phrases of this section excluding child section to playlist
        ///  This is done independent of current selection
                /// </summary>
        /// <param name="player"></param>
        /// <param name="node"></param>
        public Playlist(AudioPlayer player, ObiNode node, bool IsQaPlaylist)
        {
            resetEndOfAudioTimer();

            mPlayer = player;
            mIsQAPlaylist = IsQaPlaylist;
            Reset(LocalPlaylist);
            AddPhraseNodes(node);
        }


        /// <summary>
        /// Get the audio player for the playlist. Useful for setting up event listeners.
        /// </summary>
        public AudioPlayer Audioplayer { get { return mPlayer; } }

        public bool CanNavigatePrevPhrase { get { return true; } }
        public bool CanNavigateNextPhrase { get { return mCurrentPhraseIndex < mPhrases.Count - 1; } }
        public bool CanNavigateNextSection { get { return NextSectionIndex < mPhrases.Count - 1; } }
        public bool CanNavigateNextPage { get { return true; } }

        public PhraseNode NextPage(PhraseNode node)
        {
            int index = mPhrases.IndexOf(node) + 1;
            for (; index < mPhrases.Count && mPhrases[index].Role_ != EmptyNode.Role.Page; ++index) { }
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
            for (; index >= 0 && mPhrases[index].Role_ != EmptyNode.Role.Page; --index) { }
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
                bool playing = mPlaylistState == AudioPlayer.State.Playing;
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
            get
            {
                if (mPlayer.CurrentAudioPCMFormat == null || mPlayer.CurrentBytePosition == 0) return 0;
                return (double)mPlayer.CurrentAudioPCMFormat.ConvertBytesToTime(mPlayer.CurrentBytePosition) / Time.TIME_UNIT;
            }
            set
            {
                double phraseDurationMilliseconds =
                    mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds;

                if (value >= 0 &&
                    value < phraseDurationMilliseconds)
                {
                    AudioLibPCMFormat format = mPhrases[mCurrentPhraseIndex].Audio.AudioMediaData.PCMFormat.Data;

                    double millisecondsBegin = value;
                    long bytesBegin = format.ConvertTimeToBytes((long)(millisecondsBegin * Time.TIME_UNIT));

                    double millisecondsEnd = mPlaybackEndTime > 0.0 && millisecondsBegin < mPlaybackEndTime ? mPlaybackEndTime : phraseDurationMilliseconds;
                    long bytesEnd = format.ConvertTimeToBytes((long)(millisecondsEnd * Time.TIME_UNIT));

                    if (mPlayer.CurrentState == AudioPlayer.State.Playing)
                    {
                        mPlayer.Stop();
                        resetAudioStreamDelegate();
                        mPlayer.PlayBytes(m_StreamProviderDelegate, m_StreamProviderDelegate().Length, format, bytesBegin, bytesEnd);
                    }
                    else if (mPlayer.CurrentState == AudioPlayer.State.Paused)
                    {
                        mPlayer.Pause(bytesBegin);
                    }
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
            System.Diagnostics.Debug.Assert(mPlaylistState != AudioPlayer.State.Playing, "Only play from stopped or pause state.");
            if (mCurrentPhraseIndex < mPhrases.Count) PlayPhrase(mCurrentPhraseIndex);
        }

        public void Play(double from)
        {
        if (from < 0 || from >= mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds)
            from = 0;

            mPlaybackStartTime = from;
            Play();
        }

        public void Play(double from, double to)
        {
        if (from < 0 || from >= mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds)
            from = 0;

        if (to < 0 || to >= mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds)
            to = mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds;

            mPlaybackEndTime = to;
            Play(from);
        }

        /// <summary>
        /// Set a new presentation for this playlist; i.e., regenerate the master playlist for the presentation.
        /// </summary>
        public ObiPresentation Presentation
        {
            set
            {
                Reset(MasterPlaylist);
                if (value == null) return;
                AddPhraseNodes(value.RootNode);
                value.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                value.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
            }
        }

        /// <summary>
        /// The state of the playlist, as opposed to that of the underlying player.
        /// </summary>
        public AudioPlayer.State State { get { return mPlaylistState; } }


        // Add all phrase nodes underneath (and including) the starting node.
        // In case of the master playlist, exclude unused nodes.
        private void AddPhraseNodes(urakawa.core.TreeNode node)
        {
            if (mIsQAPlaylist)
            {
                node.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n is PhraseNode && n.Children.Count == 0 && (!mIsMaster || ((PhraseNode)n).Used))
                        {
                            mPhrases.Add((PhraseNode)n);
                            mStartTimes.Add(mTotalTime);
                            mTotalTime += ((PhraseNode)n).Audio.Duration.AsTimeSpan.TotalMilliseconds;
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
            }
            else
            {
                node.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n is PhraseNode && n.Children.Count == 0)
                        {
                            mPhrases.Add((PhraseNode)n);
                            mStartTimes.Add(mTotalTime);
                            mTotalTime += ((PhraseNode)n).Audio.Duration.AsTimeSpan.TotalMilliseconds;
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
            }
        }

        // Add phrase nodes from a strip, or a single phrase.
        // Shallow operation compared to AddPhraseNode which is deep.
        private void AddPhraseNodesFromStripOrPhrase(ObiNode node)
        {
            if (node is PhraseNode)
            {
                if (!mIsQAPlaylist || node.Used)
                {
                    mPhrases.Add((PhraseNode)node);
                    mStartTimes.Add(mTotalTime);
                    mTotalTime += ((PhraseNode)node).Audio.Duration.AsTimeSpan.TotalMilliseconds;
                }
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
            node.AcceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is PhraseNode && ((PhraseNode)n).Used)
                    {
                        double time = ((PhraseNode)n).Audio.Duration.AsTimeSpan.TotalMilliseconds;
                        mPhrases.Insert(index, (PhraseNode)n);
                        mStartTimes.Add(0.0);
                        mStartTimes[index] = index == 0 ? 0.0 :
                            (mStartTimes[index - 1] + mPhrases[index - 1].Audio.Duration.AsTimeSpan.TotalMilliseconds);
                        mTotalTime += time;
                        ++index;
                    }
                    return true;
                }, delegate(urakawa.core.TreeNode n) { }
            );
            for (int i = index; i < mStartTimes.Count - 1; ++i)
            {
                mStartTimes[i + 1] = mStartTimes[i] + mPhrases[i].Audio.Duration.AsTimeSpan.TotalMilliseconds;
            }
        }

        private AudioPlayer.StreamProviderDelegate m_StreamProviderDelegate = null;

        private void resetAudioStreamDelegate()
        {
            Stream stream = mPhrases[mCurrentPhraseIndex].Audio.AudioMediaData.OpenPcmInputStream();
            m_StreamProviderDelegate = delegate
            {
                return stream;
            };
        }

        // Play the current phrase
        private void PlayCurrentPhrase()
        {
            AudioLib.AudioPlayer.StateChangedEventArgs evargs = new AudioLib.AudioPlayer.StateChangedEventArgs(mPlayer.CurrentState);
            if (mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds == 0)
            {
                return ;
            }
                
            if (mPlaylistState == AudioPlayer.State.Stopped)
            {
                mPlayer.AudioPlaybackFinished += new AudioPlayer.AudioPlaybackFinishHandler(Playlist_MoveToNextPhrase);
            }
            mPlaylistState = AudioPlayer.State.Playing;
            double from = mPlaybackStartTime;
            mPlaybackStartTime = 0.0;
            
            //mPlayer.EnsurePlaybackStreamIsDead();
            mPlayer.Stop();
            resetAudioStreamDelegate();

            long dataLength = m_StreamProviderDelegate().Length; // format.ConvertTimeToBytes((long)(phraseDurationMilliseconds * Time.TIME_UNIT));

            AudioLibPCMFormat format = mPhrases[mCurrentPhraseIndex].Audio.AudioMediaData.PCMFormat.Data;

            double phraseDurationMilliseconds =
                    mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds;

            double millisecondsBegin = from;
            long bytesBegin = format.ConvertTimeToBytes((long)(millisecondsBegin * Time.TIME_UNIT));

            double millisecondsEnd = mPlaybackEndTime > 0.0 ? mPlaybackEndTime : phraseDurationMilliseconds;
            long bytesEnd = format.ConvertTimeToBytes((long)(millisecondsEnd * Time.TIME_UNIT));

            if (mCurrentPhraseIndex == mPhrases.Count - 1 && mPlaybackEndTime > 0.0)
            {
                mPlayer.PlayBytes(m_StreamProviderDelegate, dataLength, format, bytesBegin, bytesEnd);
            }
            else
            {
                mPlayer.PlayBytes(m_StreamProviderDelegate, dataLength, format, bytesBegin, bytesEnd);
            }

            // send the state change event if the state actually changed
            if (StateChanged != null && mPlayer.CurrentState != evargs.OldState) StateChanged(this, evargs);
        }

        // Play the phrase at the given index in the list.
        protected void PlayPhrase(int index)
        {
            SkipToPhrase(index);
            PlayCurrentPhrase();
        }

        // React to addition and removal of tree nodes in the presentation.
        private void Presentation_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (e is ObjectAddedEventArgs<urakawa.core.TreeNode>)
            {
                InsertNode(((ObjectAddedEventArgs<urakawa.core.TreeNode>)e).m_AddedObject);
            }
            else if (e is ObjectRemovedEventArgs<urakawa.core.TreeNode>)
            {
                RemoveNode(((ObjectRemovedEventArgs<urakawa.core.TreeNode>)e).m_RemovedObject);
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
            node.AcceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is PhraseNode && mPhrases.Contains((PhraseNode)n))
                    {
                        int index = mPhrases.IndexOf((PhraseNode)n);
                        if (updateTimeFrom == mPhrases.Count) updateTimeFrom = index == 0 ? 1 : index;
                        mPhrases.RemoveAt(index);
                        if (index < mStartTimes.Count - 1) mStartTimes.RemoveAt(index + 1);
                        mTotalTime -= ((PhraseNode)n).Audio.Duration.AsTimeSpan.TotalMilliseconds ;
                    }
                    return true;
                },
                delegate(urakawa.core.TreeNode n) { }
            );
            for (int i = updateTimeFrom; i < mPhrases.Count; ++i)
            {
                mStartTimes[i] = mStartTimes[i - 1] + mPhrases[i - 1].Audio.Duration.AsTimeSpan.TotalMilliseconds;
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
            mPlaylistState = mPlayer.CurrentState;
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
                    mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds : 0.0;
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
                //mPlayer.CurrentTimePosition
                double currentTimePosition =
                      (double)mPlayer.CurrentAudioPCMFormat.ConvertBytesToTime(mPlayer.CurrentBytePosition) /
                      Time.TIME_UNIT;

                return mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds - currentTimePosition;
            }
            set
            {
                CurrentTimeInAsset = mPhrases[mCurrentPhraseIndex].Audio.Duration.AsTimeSpan.TotalMilliseconds - value;
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
            System.Diagnostics.Debug.Assert(mPlaylistState == AudioPlayer.State.Paused, "Only resume from paused state.");
            mPlaylistState = AudioPlayer.State.Playing;
            mPlayer.Resume();
            // TODO: mPlayer.Play(mPhrases[mCurrentPhraseIndex].Asset, mPausePosition);
            if (StateChanged != null)
            {
                StateChanged(this, new AudioLib.AudioPlayer.StateChangedEventArgs(AudioPlayer.State.Paused));
            }
        }

        /// <summary>
        /// Catch the end of an asset from the audio player and move to the next phrase.
        /// </summary>
        /// <param name="sender">Sender of the event (i.e. the audio player.)</param>
        /// <param name="e">The arguments sent by the player.</param>
        protected virtual void Playlist_MoveToNextPhrase(object sender, AudioPlayer.AudioPlaybackFinishEventArgs e)
        {
            //if (true) xxx
            //{
            //    Playlist_MoveToNextPhrase();
            //    return;
            //}
            //m_FinishedPlayingCurrentStream = true;
                    //m_MonitoringTimer.Enabled = true;
            //m_MonitoringTimer.Start();

            if (mPlayer.OutputDeviceControl.InvokeRequired)
            {
                mPlayer.OutputDeviceControl.Invoke(new Playlist_MoveToNextPhrase_Delegate(Playlist_MoveToNextPhrase));
            }
            else
            {
                Playlist_MoveToNextPhrase();
            }
        }
        
        protected delegate  void  Playlist_MoveToNextPhrase_Delegate();

        //private System.Windows.Forms.Timer m_MonitoringTimer = null;
        //private System.Timers.Timer  m_MonitoringTimer = new System.Timers.Timer();
        //private bool m_FinishedPlayingCurrentStream = false;

        private void resetEndOfAudioTimer()
        {
        //    if (m_MonitoringTimer!=null)
        //    {
        //        m_MonitoringTimer.Stop();
        //        m_MonitoringTimer.Tick -= new EventHandler(m_MonitoringTimer_Tick);
        //    }
        //    m_MonitoringTimer = new System.Windows.Forms.Timer();
        //    m_MonitoringTimer.Tick += new EventHandler(m_MonitoringTimer_Tick);
        //    m_MonitoringTimer.Interval = 50;
        //    m_MonitoringTimer.Enabled = false;


            //if (m_MonitoringTimer != null)
            //{
            //    m_MonitoringTimer.Stop();
            //    m_MonitoringTimer.Elapsed -= new EventHandler(m_MonitoringTimer_Tick);
            //}
            //m_MonitoringTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_MonitoringTimer_Tick);
            //m_MonitoringTimer.Interval = 50;
            //m_MonitoringTimer.AutoReset = true;
        }

        //private void m_MonitoringTimer_Tick(object sender, EventArgs e)
        //{
        //    //Console.WriteLine("monitoring ");
        //    if ( m_FinishedPlayingCurrentStream)
        //    {
        //        if (m_MonitoringTimer != null)
        //        {
        //            m_MonitoringTimer.Stop();
        //            //m_MonitoringTimer.Enabled = false;
        //        }
        //        m_FinishedPlayingCurrentStream = false;
        //        Playlist_MoveToNextPhrase();
        //    }
        //}
        private void Playlist_MoveToNextPhrase()
        {
            if (mPlayer.PlaybackFwdRwdRate < 0 && mCurrentPhraseIndex > 0)
            {
                // Going backward so play previous phrase
                PlayPhrase(mCurrentPhraseIndex - 1);
            }
            else if (mPlayer.PlaybackFwdRwdRate >= 0 && mCurrentPhraseIndex < mPhrases.Count - 1)
            {
                // Going forward so play next phrase
                PlayPhrase(mCurrentPhraseIndex + 1);
            }
            else
            {
                ReachedEndOfPlaylist();
            }
        }

        // What to do when the end of the playlist has been reached?
        // In the normal case we just stop.
        protected virtual void ReachedEndOfPlaylist()
        {
            //Audioplayer.AudioPlaybackFinished -= new AudioPlayer.AudioPlaybackFinishHandler(Playlist_MoveToNextPhrase);
            Stop();
            if (EndOfPlaylist != null) EndOfPlaylist(this, new EventArgs());
        }

        /// <summary>
        /// Pause.
        /// </summary>
        public virtual void Pause()
        {
            if (mPlaylistState == AudioPlayer.State.Playing)
            {
                            mPlaylistState = AudioPlayer.State.Paused;
                mPlayer.Pause();
                mPlayer.PlaybackFwdRwdRate = 0;
                if (StateChanged != null)
                {
                    StateChanged(this, new AudioLib.AudioPlayer.StateChangedEventArgs(AudioPlayer.State.Playing));
                }
            }
        }

        public void PauseFromStopped(double time)
        {
            AudioLibPCMFormat format = mPhrases[mCurrentPhraseIndex].Audio.AudioMediaData.PCMFormat.Data;
            long bytes = format.ConvertTimeToBytes((long)(time * Time.TIME_UNIT));

            mPlayer.Pause(bytes);
            Console.WriteLine("Player state in playlist class " + mPlayer.CurrentState);
            mPlaylistState = AudioPlayer.State.Paused;
            Console.WriteLine("playlist " + mPlaylistState);
            if (StateChanged != null)
            {
                StateChanged(this, new AudioLib.AudioPlayer.StateChangedEventArgs(AudioPlayer.State.Stopped));
                Console.WriteLine("playlist 1" + mPlaylistState);
            }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            if (mPlaylistState == AudioPlayer.State.Playing || mPlaylistState == AudioPlayer.State.Paused)
            {
                AudioLib.AudioPlayer.StateChangedEventArgs evargs = new AudioLib.AudioPlayer.StateChangedEventArgs(mPlayer.CurrentState);
                mPlaylistState = AudioPlayer.State.Stopped;
                mPlayer.PlaybackFwdRwdRate = 0;
                mCurrentPhraseIndex = 0;
                mElapsedTime = 0.0;

                mPlayer.AudioPlaybackFinished -= new AudioPlayer.AudioPlaybackFinishHandler(Playlist_MoveToNextPhrase);

                mPlayer.Stop();
                
                if (StateChanged != null) StateChanged(this, evargs);
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

                AudioLib.AudioPlayer.StateChangedEventArgs evargs = new AudioLib.AudioPlayer.StateChangedEventArgs ( mPlayer.CurrentState );
                /*commented for AudioLib migration
                if (mPlayer.CurrentState == AudioPlayer.State.Paused)
                    mPlayer.Resume();
                else if (mPlayer.CurrentState == AudioPlayer.State.Stopped)
                    Play();
                */
                if (mPlayer.CurrentState == AudioPlayer.State.Playing)
                    mPlaylistState = AudioPlayer.State.Playing;

                mPlayBackState = PlayBackState.Rewind;
                if (StateChanged != null && mPlayer.CurrentState != evargs.OldState) StateChanged ( this, evargs );
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
                AudioLib.AudioPlayer.StateChangedEventArgs evargs = new AudioLib.AudioPlayer.StateChangedEventArgs ( mPlayer.CurrentState );
                /* commented for AudioLib migration
                if (mPlayer.CurrentState == AudioPlayer.State.Paused)
                    mPlayer.Resume();
                else if (mPlayer.CurrentState == AudioPlayer.State.Stopped)
                    Play();
                */
                if (mPlayer.CurrentState == AudioPlayer.State.Playing)
                    mPlaylistState = AudioPlayer.State.Playing;

                mPlayBackState = PlayBackState.Forward;
                if (StateChanged != null && mPlayer.CurrentState != evargs.OldState) StateChanged ( this, evargs );
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
                    && n.Role_ != EmptyNode.Role.Page)
                {
                    --PrevPagePhraseIndex;
                    n = (PhraseNode)mPhrases[PrevPagePhraseIndex];
                }

                if (PrevPagePhraseIndex < mCurrentPhraseIndex && PrevPagePhraseIndex >= 0
                    &&     n.Role_ == EmptyNode.Role.Page )
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
                double currentTimePosition =
                     (double)mPlayer.CurrentAudioPCMFormat.ConvertBytesToTime(mPlayer.CurrentBytePosition) /
                     Time.TIME_UNIT;

                double currentTime = mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing ? currentTimePosition : 0.0;
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
                    && n.Role_ != EmptyNode.Role.Page)
                {
                    ++NextPagePhraseIndex;
                    n = (PhraseNode)mPhrases[NextPagePhraseIndex];
                }

                if ( NextPagePhraseIndex > mCurrentPhraseIndex && NextPagePhraseIndex < mPhrases.Count
                    &&     n.Role_ == EmptyNode.Role.Page )
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
            if (mPlaylistState == AudioPlayer.State.Playing)
            {
                mPlayer.Stop();
                PlayPhrase(index);
            }
            else if (mPlaylistState == AudioPlayer.State.Paused)
            {
                SkipToPhrase(index);
            }
            else if (mPlaylistState == AudioPlayer.State.Stopped)
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
                mStartTimes[i] = mStartTimes[i - 1] + mPhrases[i - 1].Audio.Duration.AsTimeSpan.TotalMilliseconds;
            }
            mTotalTime = mStartTimes[mStartTimes.Count - 1] + mPhrases[mStartTimes.Count - 1].Audio.Duration.AsTimeSpan.TotalMilliseconds;
            System.Diagnostics.Debug.Print("!!! Playlist: {0} phrase(s), length = {1}ms.", mPhrases.Count, mTotalTime);
        }

        public bool ContainsPhrase(PhraseNode phrase)
        {
            return phrase != null && mPhrases.Contains(phrase);
        }

        public void FastPlayNormaliseWithLapseBack(double LapseBackTime)
        {
            double currentTimePosition =
                     (double)mPlayer.CurrentAudioPCMFormat.ConvertBytesToTime(mPlayer.CurrentBytePosition) /
                     Time.TIME_UNIT;

            bool wasPaused = State == AudioPlayer.State.Paused;
            //Console.WriteLine("paused time before " + mPlayer.CurrentTimePosition);
            if (currentTimePosition > LapseBackTime)
            {
                mPlayer.Pause();
                mPlayer.FastPlayFactor = 1;
                CurrentTimeInAsset = currentTimePosition - LapseBackTime;
            }
            else
            {
                mPlayer.Pause();
                CurrentTimeInAsset = 10;
                mPlayer.FastPlayFactor = 1;
            }
            //Console.WriteLine("paused time " + mPlayer.CurrentTimePosition);
            if (!wasPaused)
            {
                mPlaylistState = AudioPlayer.State.Playing;
                mPlayer.Resume();
                if (StateChanged != null)
                    StateChanged(this, new AudioLib.AudioPlayer.StateChangedEventArgs(AudioPlayer.State.Paused));
            }
        }




    }


    /// <summary>
    /// Special playlist used for preview. It reverts to pause at the beginning after playback is done.
    /// If we get both end of audio asset/stopped event, then we can revert.
    /// </summary>
    public class PreviewPlaylist : Playlist
    {
        private double mRevertTime;  // revert time

        public PreviewPlaylist(AudioPlayer player, NodeSelection selection, double revertTime)
            : base(player, selection, false)
        {
            base.Audioplayer.AudioPlaybackFinished -= new AudioPlayer.AudioPlaybackFinishHandler(base.Playlist_MoveToNextPhrase);
            base.Audioplayer.AudioPlaybackFinished += new AudioPlayer.AudioPlaybackFinishHandler(Playlist_MoveToNextPhrase);
            mRevertTime = revertTime;
        }

        public PreviewPlaylist(AudioPlayer player, ObiNode node, double revertTime)
            : base(player, node, false)
        {
            base.Audioplayer.AudioPlaybackFinished -= new AudioPlayer.AudioPlaybackFinishHandler(base.Playlist_MoveToNextPhrase);
            base.Audioplayer.AudioPlaybackFinished += new AudioPlayer.AudioPlaybackFinishHandler(Playlist_MoveToNextPhrase);
            mRevertTime = revertTime;
        }

        public double RevertTime { get { return mRevertTime; } }

        public override void Pause()
            {
                if (base.State == AudioPlayer.State.Playing)
                    {
                    mRevertTime = base.CurrentTimeInAsset ;
                    base.Pause () ;
                    }
                }


        protected override void Playlist_MoveToNextPhrase(object sender, AudioPlayer.AudioPlaybackFinishEventArgs e)
            {
            CallEndOfPreviewPlaylist ();
            }


        public void TriggerEndOfPreviewPlaylist ( double time   )
            {
            if (time > 0 && time <
                mPhrases[mCurrentPhraseIndex].Audio.AudioMediaData.AudioDuration.AsTimeSpan.TotalMilliseconds
                //Audioplayer.CurrentAudio.AudioDuration.AsTimeSpan.TotalMilliseconds
                )
                {
                mRevertTime = time;
                CallEndOfPreviewPlaylist ();
                }
            }

                private  void CallEndOfPreviewPlaylist ()
            {
            if (base.Audioplayer.PlaybackFwdRwdRate < 0 && mCurrentPhraseIndex > 0)
                {
                // Going backward so play previous phrase
                PlayPhrase ( mCurrentPhraseIndex - 1 );
                }
            else if (base.Audioplayer.PlaybackFwdRwdRate >= 0 && mCurrentPhraseIndex < mPhrases.Count - 1)
                {
                // Going forward so play next phrase
                PlayPhrase ( mCurrentPhraseIndex + 1 );
                }
            else
                {
                ReachedEndOfPlaylist ();
                }
            }

        protected override void ReachedEndOfPlaylist()
        {
            base.Audioplayer.AudioPlaybackFinished -= new AudioPlayer.AudioPlaybackFinishHandler(Playlist_MoveToNextPhrase);
            PauseFromStopped(mRevertTime);
        }
    }
}