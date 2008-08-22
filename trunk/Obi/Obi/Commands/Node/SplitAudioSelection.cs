using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data.audio;
using urakawa.media.timing;


namespace Obi.Commands.Node
{
    class SplitAudioSelection : Command
    {
        private PhraseNode mOriginalNode;
        private PhraseNode mMiddleNode ;
        private PhraseNode mLastNode;
        private double mSplitTimeBegin;
        private double mSplitTimeEnd;


        public SplitAudioSelection(ProjectView.ProjectView  view)
            : base(view)
        {
            mOriginalNode = view.Selection.Phrase;
            mSplitTimeBegin = ((AudioSelection)view.Selection).AudioRange.SelectionBeginTime;
            mSplitTimeEnd = ((AudioSelection)view.Selection).AudioRange.SelectionEndTime;
        }

        public static PhraseNode Split(PhraseNode node, Time splitPoint)
        {
            PhraseNode newNode = ((Presentation)node.getPresentation()).CreatePhraseNode(node.SplitAudio(splitPoint));
            node.InsertAfterSelf(newNode);
            return newNode;
        }

        public override void execute()
        {
            mLastNode = mSplitTimeEnd < mOriginalNode.Duration ? Split(mOriginalNode, new Time(mSplitTimeEnd)) : null;
            mMiddleNode = mSplitTimeBegin > 0 ? Split(mOriginalNode, new Time(mSplitTimeBegin)) : mOriginalNode;
            View.SelectedBlockNode = mMiddleNode;
        }

        public override void unExecute()
        {
            if (mOriginalNode != mMiddleNode) MergeAudio.Merge(mOriginalNode, mMiddleNode);
            if (mLastNode != null) MergeAudio.Merge(mMiddleNode, mLastNode );
            base.unExecute();
        }
    }
}
