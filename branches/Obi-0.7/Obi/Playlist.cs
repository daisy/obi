using Obi.Assets;
using Obi.Audio;
using System.Collections.Generic;
using urakawa.core;

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
        private Project mProject;            // project in which we are playing
        private AudioPlayer mPlayer;         // audio player for actually playing
        private List<CoreNode> mPhraseList;  // list of phrase nodes (from which we get the assets)
        private int mCurrentPhrase;          // index of the phrase currently playing
        private double mTotalTime;           // total time of this playlist
        private double mElapsedTime;         // elapsed time *before* the beginning of the current asset
        private bool mWholeBook;             // flag for playing whole book or just a selection

        // Amount of time after which "previous phrase" goes to the beginning of the phrase
        // rather than the actual previous phrase. In milliseconds.
        private static readonly double InitialThreshold = 2500.0;

        /// <summary>
        /// Get the phrase currently playing.
        /// </summary>
        public CoreNode CurrentPhrase
        {
            get { return mPhraseList[mCurrentPhrase]; }
        }

        /// <summary>
        /// Get the next phrase in play order. Return null if we are at the end.
        /// </summary>
        public CoreNode NextPhrase
        {
            get { return mCurrentPhrase < mPhraseList.Count - 1 ? mPhraseList[mCurrentPhrase + 1] : null; }
        }

        /// <summary>
        /// Get the previous phrase in play order. Return null if this is the first phrase,
        /// and same phrase if we are past the initial threshold.
        /// </summary>
        public CoreNode PreviousPhrase
        {
            get
            {
                if (mCurrentPhrase > 0)
                {
                    AudioMediaAsset current = mPlayer.CurrentAsset;
                    if (current == null) current = Project.GetAudioMediaAsset(mPhraseList[mCurrentPhrase]);
                    // adapt the initial threshold to the length of the asset
                    double threshold = InitialThreshold > current.LengthInMilliseconds / 2.0 ?
                        InitialThreshold : current.LengthInMilliseconds / 2.0;
                    System.Diagnostics.Debug.Assert(mPlayer.State != AudioPlayerState.NotReady);
                    // really go back only if stopped or if before the end of the initial threshold
                    bool goback = mPlayer.State == AudioPlayerState.Stopped || mPlayer.CurrentTimePosition <= threshold;
                    return mPhraseList[mCurrentPhrase - (goback ? 1 : 0)];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The section in which the currently playing phrase is.
        /// </summary>
        public CoreNode CurrentSection
        {
            get { return (CoreNode)CurrentPhrase.getParent(); }
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
                int i = mCurrentPhrase + 1;
                for (; i < mPhraseList.Count && mPhraseList[i].getParent() == current; ++i) { }
                return i < mPhraseList.Count ? mPhraseList[i] : null;
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
                System.Diagnostics.Debug.Assert(mPlayer.State == AudioPlayerState.Paused ||
                    mPlayer.State == AudioPlayerState.Playing);
                return mElapsedTime + mPlayer.GetCurrentTimePosition();
            }
            set
            {
                // TODO
                // check that the time is within bounds
                // find which phrase it happens in
                // if playing, resume to this position
                // if stopped, start playing from that position
                // if paused, move to that position but stay paused
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
            mPhraseList = new List<CoreNode>();
            mCurrentPhrase = 0;
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
                        mPhraseList.Add(node);
                        mTotalTime += Project.GetAudioMediaAsset(node).LengthInMilliseconds;
                    }
                    return true;
                },
                // nothing to do in post-visit
                delegate(ICoreNode n) { }
            );
        }

        /// <summary>
        /// Start playing or resume playing after pausing.
        /// </summary>
        public void Play()
        {
            System.Diagnostics.Debug.Assert(mPhraseList.Count > 0, "Phrase list is empty.");
            if (mCurrentPhrase == -1)
            {
                System.Diagnostics.Debug.Assert(mPlayer.State == AudioPlayerState.Stopped, "Player should be stopped.");
                // stopped, so start from the beginning
                mCurrentPhrase = 0;
                mElapsedTime = 0.0;
                // setup the event handler for the end of an asset (so that we can move to the next one.)
                mPlayer.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(EndOfAudioAsset);
            }
            else
            {
                System.Diagnostics.Debug.Assert(mPlayer.State == AudioPlayerState.Paused, "Player should be paused.");
                mPlayer.Resume();
            }
        }

        /// <summary>
        /// Catch the end of an asset from the audio player.
        /// </summary>
        /// <param name="sender">Sender of the event (i.e. the audio player.)</param>
        /// <param name="e">The arguments sent by the player.</param>
        private void EndOfAudioAsset(object sender, Events.Audio.Player.EndOfAudioAssetEventArgs e)
        {
            // needs to stop here?
            // add an option to have a pause between assets
            if (NextPhrase != null)
            {
                PlayNextPhrase();
            }
            else
            {
                Stop();
            }
        }

        private void PlayNextPhrase()
        {
            MoveToNextPhrase();
            mPlayer.Play(Project.GetAudioMediaAsset(mPhraseList[mCurrentPhrase]));
        }

        private void MoveToNextPhrase()
        {
            mElapsedTime += mPlayer.CurrentAsset.LengthInMilliseconds;
            ++mCurrentPhrase;
        }

        /// <summary>
        /// Pause.
        /// </summary>
        public void Pause()
        {
            System.Diagnostics.Debug.Assert(mCurrentPhrase >= 0, "Nothing currently playing.");
            System.Diagnostics.Debug.Assert(mPlayer.State == AudioPlayerState.Playing, "Player is not playing.");
            mPlayer.Pause();
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            if (mPlayer.State != AudioPlayerState.Stopped)
            {
                System.Diagnostics.Debug.Assert(mCurrentPhrase >= 0, "Nothing currently playing.");
                System.Diagnostics.Debug.Assert(mPlayer.State == AudioPlayerState.NotReady, "Player is not ready.");
                mPlayer.Stop();
                mPlayer.EndOfAudioAsset -= new Events.Audio.Player.EndOfAudioAssetHandler(EndOfAudioAsset);
                mCurrentPhrase = -1;
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
            if (NextPhrase != null)
            {
                if (mPlayer.State == AudioPlayerState.Playing)
                {
                    PlayNextPhrase();
                }
                else if (mPlayer.State == AudioPlayerState.Paused)
                {
                    MoveToNextPhrase();
                }
                else if (mPlayer.State == AudioPlayerState.Stopped)
                {
                }
            }
            else
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
    }
}
