using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Commands.Strips
{
    public class AddPhrase: Command
    {
        private Obi.Project mProject;  // the current project
        private CoreNode mNode;        // the phrase node to add/remove
        private CoreNode mParent;      // the section node to add to
        private int mIndex;            // position within the parent

        public override string Label
        {
            get { return Localizer.Message("add_phrase_command_label"); }
        }

        public AddPhrase(Obi.Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mParent = (CoreNode)mNode.getParent();
            mIndex = mParent.indexOf(mNode);
        }

        /// <summary>
        /// Do: add the node to the project; it will send the synchronization events.
        /// This is really redo, so the node exists and so does its parent.
        /// </summary>
        public override void Do()
        {
            mProject.AddPhraseNodeAndAsset(mNode, mParent, mIndex);
        }

        /// <summary>
        /// Undo: remove the node from the project; it will send the synchronization events.
        /// This node has no descendant when we undo.
        /// </summary>
        public override void Undo()
        {
            mProject.DeletePhraseNodeAndAsset(mNode);
        }
    }

    public class PastePhrase : AddPhrase
    {
        public override string Label
        {
            get { return Localizer.Message("paste_phrase_command_label");  }
        }

        public PastePhrase(Obi.Project project, CoreNode node)
            : base(project, node)
        {
        }
    }
}
