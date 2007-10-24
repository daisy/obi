using System;
using System.Collections.Generic;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// Import phrases from files.
    /// </summary>
    class ImportPhrases: Command
    {
        private ObiNode mParent;            // parent node for the phrases
        private int mFirstIndex;            // index of the first phrase to import
        private List<PhraseNode> mPhrases;  // the created phrases

        public ImportPhrases(ProjectView.ProjectView view, List<PhraseNode> phrases)
            : base(view)
        {
            // normally we will want the dummy phrase to be selected
            if (SelectionBefore.Node is SectionNode)
            {
                mParent = SelectionBefore.Node;
                mFirstIndex = 0;
            }
            else
            {
                // after the selected node
                mParent = SelectionBefore.Node.ParentAs<ObiNode>();
                mFirstIndex = SelectionBefore.Node.Index + 1;
            }
            mPhrases = phrases;
        }

        public override string getShortDescription() { return Localizer.Message("import_phrases_command"); }

        public override void execute()
        {
            base.execute();
            for (int i = 0; i < mPhrases.Count; ++i) mParent.Insert(mPhrases[i], mFirstIndex + i);
            View.Selection = new NodeSelection(mPhrases[mPhrases.Count - 1], SelectionBefore.Control, false);
        }

        public override void unExecute()
        {
            foreach (PhraseNode phrase in mPhrases) mParent.RemoveChild(phrase);
            base.unExecute();
        }
    }
}
