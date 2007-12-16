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
            mParentSection = view.SelectedNodeAs<EmptyNode>().ParentAs<SectionNode>();
            mOriginalPhraseIndex =  mOriginalPhrase.ParentAs<ObiNode>().Index;
            mPresentation = view.Presentation;
            mThreshold = threshold;
            mGap = gap;
            mBeforePhraseSilence = before;
        }


        public override void execute()
        {
                        mDetectedPhrases = mPresentation.CreatePhraseNodesFromAudioAssetList (   Audio.PhraseDetection.Apply ( mOriginalPhrase.Audio , mThreshold , mGap , mBeforePhraseSilence ) ) ;
            mParentSection.RemoveChildPhrase(mOriginalPhrase);

            for (int i = 0; i < mDetectedPhrases.Count; i++)
            {
                mParentSection.Insert(mDetectedPhrases [i], mOriginalPhraseIndex + i);
            }

        }

        public override void unExecute()
        {
            for (int i = 0; i < mDetectedPhrases.Count; i++)
            {
                mParentSection.RemoveChildPhrase(mDetectedPhrases[i]);
            }
            mParentSection.Insert(mOriginalPhrase, mOriginalPhraseIndex);
        }

    }
}
