using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    class DeletePhrase: Command
    {
        private Obi.Project mProject;  // the current project
        private CoreNode mNode;        // the phrase node to add/remove
        private CoreNode mParent;      // the section node to add to
        private int mIndex;            // position within the parent

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
            mProject.DeletePhraseNode(mNode);
        }

        public override void Undo()
        {
            mProject.AddExistingPhrase(mNode, mParent, mIndex);
        }
    }
}
