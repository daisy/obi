using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    class DeletePhrase: Command
    {
        protected Obi.Project mProject;  // the current project
        protected CoreNode mNode;        // the phrase node to add/remove
        private CoreNode mParent;        // the section node to add to
        private int mIndex;              // position within the parent

        public override string Label
        {
            get { return Localizer.Message("delete_phrase_command_label"); }
        }

        public DeletePhrase(Project project, CoreNode node, CoreNode parent, int index)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
        }
        
        public override void Do()
        {
            mProject.DeletePhraseNodeAndAsset(mNode);
        }

        public override void Undo()
        {
            mProject.AddPhraseNodeAndAsset(mNode, mParent, mIndex);
        }
    }

    class CutPhrase : DeletePhrase
    {
        private CoreNode mPrevClipBoard;  // previous clip board block

        public override string Label
        {
            get { return Localizer.Message("cut_phrase_command_label"); }
        }

        public CutPhrase(Project project, CoreNode node, CoreNode parent, int index)
            : base(project, node, parent, index)
        {
            mPrevClipBoard = project.BlockClipBoard;
        }

        public override void Do()
        {
            base.Do();
            mProject.BlockClipBoard = mNode;
        }

        public override void Undo()
        {
            base.Undo();
            mProject.BlockClipBoard = mPrevClipBoard;
        }
    }
}
