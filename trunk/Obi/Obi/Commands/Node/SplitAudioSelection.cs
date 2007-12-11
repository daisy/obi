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
        private double mSplitTime1;
        private double mSplitTime2;


        public SplitAudioSelection(ProjectView.ProjectView  view)
            : base(view)
        {
            mOriginalNode = view.Selection.Phrase ;
            mSplitTime1 = view.Selection.Waveform.SelectionBeginTime;
            mSplitTime2 =view.Selection.Waveform.SelectionEndTime  ;
        }

        public static PhraseNode Split(PhraseNode node, Time splitPoint)
        {
            PhraseNode newNode = ((Presentation)node.getPresentation()).CreatePhraseNode(node.SplitAudio(splitPoint));
            node.InsertAfterSelf(newNode);
            return newNode;
        }

        public override void execute()
        {
            base.execute();
            mMiddleNode = Split(mOriginalNode , new Time ( mSplitTime1 ) ) ;
            mLastNode = Split( mMiddleNode , new Time ( mSplitTime2- mSplitTime1  ) );
            View.SelectedBlockNode = mMiddleNode ;
        }
        public override void unExecute()
        {
            MergeAudio.Merge(mOriginalNode , mMiddleNode );
            MergeAudio.Merge(mMiddleNode , mLastNode );
            base.unExecute();
        }

    }
}
