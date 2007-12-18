using System;
using System.Collections.Generic;
using System.Text;

using Obi.Commands;
namespace Obi.Commands.Node
{
    public class PhraseDetection: Command
    {
        private PhraseNode mOriginalPhrase ;
        private SectionNode mParentSection;
        private int mOriginalPhraseIndex ;
        private List<PhraseNode>  mDetectedPhrases ;
        private  Presentation mPresentation;

        private long mThreshold;
        private double mGap;
        private double mBeforePhraseSilence ;


        public PhraseDetection(ProjectView.ProjectView view  , long  threshold , double gap , double before )
            : base( view )
        {
            mOriginalPhrase = view.SelectedNodeAs<PhraseNode>();
            mParentSection = mOriginalPhrase.ParentAs<SectionNode>();
            mOriginalPhraseIndex = mOriginalPhrase.Index;
            mPresentation = view.Presentation;
            mThreshold = threshold;
            mGap = gap;
            mBeforePhraseSilence = before;
        }


        public override void execute()
        {
            mDetectedPhrases = mPresentation.CreatePhraseNodesFromAudioAssetList(Obi.Audio.PhraseDetection.Apply(mOriginalPhrase.Audio, mThreshold, mGap, mBeforePhraseSilence));
            mOriginalPhrase.Detach();
            for (int i = 0; i < mDetectedPhrases.Count; i++)
            {
                mParentSection.Insert(mDetectedPhrases[i], mOriginalPhraseIndex + i);
            }
            base.execute();
        }

        public override void unExecute()
        {
            for (int i = 0; i < mDetectedPhrases.Count; i++) mDetectedPhrases[i].Detach();
            mParentSection.Insert(mOriginalPhrase, mOriginalPhraseIndex);
            base.unExecute();
        }

    }
}
