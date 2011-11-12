using System;
using System.Collections.Generic;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;
using urakawa.media.data.audio;
using urakawa.media.data;

namespace Obi.Commands.Audio
{
    class Delete : Command
    {
        private PhraseNode mNode;               // the phrase node in which the deletion happens
        private PhraseNode mDeleted;            // node to store the deleted audio (unrooted)
        private Time mSplitTimeBegin;           // begin time of audio split
        private Time mSplitTimeEnd;             // end time of audio split
        private NodeSelection mSelectionAfter;  // selection after deletion (cursor at split point)
        private bool mHasAudioAfterDeleted;     // true if there is audio left after the deleted part

        // Made private so that GetCommand() is used instead
        private Delete(ProjectView.ProjectView view)
            : base(view)
        {
            mNode = (PhraseNode)view.Selection.Node;
            mHasAudioAfterDeleted =
                ((AudioSelection)view.Selection).AudioRange.SelectionEndTime < mNode.Audio.Duration.AsTimeSpan.TotalMilliseconds;
            mSplitTimeBegin = new Time((long)(((AudioSelection)view.Selection).AudioRange.SelectionBeginTime * Time.TIME_UNIT));
            mSplitTimeEnd = new Time((long)(((AudioSelection)view.Selection).AudioRange.SelectionEndTime * Time.TIME_UNIT));
            mSelectionAfter = mHasAudioAfterDeleted ?
                new AudioSelection(mNode, view.Selection.Control, new AudioRange(mSplitTimeBegin.AsTimeSpan.TotalMilliseconds)) :
                new NodeSelection(mNode, view.Selection.Control);

            ManagedAudioMedia manMedia = view.Presentation.MediaFactory.CreateManagedAudioMedia();
            WavAudioMediaData mediaData = ((WavAudioMediaData) mNode.Audio.AudioMediaData).Copy(mSplitTimeBegin, mSplitTimeEnd);
            manMedia.AudioMediaData = mediaData;

            mDeleted = view.Presentation.CreatePhraseNode(manMedia);
            SetDescriptions(Localizer.Message("delete_audio"));
        }

        public PhraseNode Deleted { get { return mDeleted; } }

        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                List<MediaData> mediaList = new List<MediaData>();
                if (mDeleted != null && mDeleted is PhraseNode && mDeleted.Audio != null)
                    mediaList.Add(mDeleted.Audio.MediaData);

                return mediaList;
            }
        }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            ManagedAudioMedia after = mHasAudioAfterDeleted ? mNode.SplitAudio(mSplitTimeEnd) : null;
            mNode.SplitAudio(mSplitTimeBegin);
            if (after != null) mNode.MergeAudioWith(after);
            View.Selection = mSelectionAfter;
        }

        public override void UnExecute()
        {
            ManagedAudioMedia after = mHasAudioAfterDeleted ? mNode.SplitAudio(mSplitTimeBegin) : null;
            mNode.MergeAudioWith(mDeleted.Audio.Copy());
            if (after != null) mNode.MergeAudioWith(after);
            base.UnExecute();
        }

        /// <summary>
        /// Get a delete command handling the delete all audio case gracefully.
        /// </summary>
        public static urakawa.command.Command GetCommand(Obi.ProjectView.ProjectView view)
        {
            Delete command = new Delete(view);
            if (!command.mHasAudioAfterDeleted && command.mSplitTimeBegin.AsTimeSpan.TotalMilliseconds == 0.0)
            {
                // Delete the whole audio
                urakawa.command.CompositeCommand composite =
                    view.Presentation.CreateCompositeCommand(command.ShortDescription);
                EmptyNode empty = view.Presentation.TreeNodeFactory.Create<EmptyNode>();
                composite.ChildCommands.Insert(composite.ChildCommands.Count,
                    new Commands.Node.AddEmptyNode(view, empty, command.mNode.ParentAs<ObiNode>(),
                    command.mNode.Index)
                    );
                Commands.Node.MergeAudio.AppendCopyNodeAttributes(composite, view, command.mNode, empty);
                Commands.Node.Delete delete = new Commands.Node.Delete(view, command.mNode);
                delete.UpdateSelection = false;
                composite.ChildCommands.Insert(composite.ChildCommands.Count, delete);
                return composite;
            }
            else
            {
                return command;
            }
        }
    }
}
