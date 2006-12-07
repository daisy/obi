using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// Command to delete a phrase. Undo restores the phrase in its place.
    /// The command must be created before the node is actually deleted.
    /// </summary>
    public class DeletePhrase: Command
    {
        private PhraseNode mNode;     // the phrase node to add/remove
        private SectionNode mParent;  // the section node to add to
        private int mIndex;           // original index of the phrase

        protected PhraseNode Node
        {
            get { return mNode; }
        }

        public override string Label
        {
            get { return Localizer.Message("delete_phrase_command_label"); }
        }

        public DeletePhrase(PhraseNode node)
        {
            mNode = node;
            mParent = node.ParentSection;
            mIndex = node.Index;
        }
        
        public override void Do()
        {
            mNode.Project.DeletePhraseNodeAndAsset(mNode);
        }

        public override void Undo()
        {
            mNode.Project.AddPhraseNodeAndAsset(mNode, mParent, mIndex);
        }
    }

    /// <summary>
    /// Cut is like delete except that the deleted node is stored in the clipboard.
    /// Previous values are saved so that undo restores the previous clipboard value.
    /// </summary>
    class CutPhrase : DeletePhrase
    {
        private PhraseNode mPrevBlockClipBoard;  // previous clip board block

        public override string Label
        {
            get { return Localizer.Message("cut_phrase_command_label"); }
        }

        public CutPhrase(PhraseNode node)
            : base(node)
        {
            mPrevBlockClipBoard = Node.Project.Clipboard.Phrase;
        }

        public override void Do()
        {
            base.Do();
            Node.Project.Clipboard.Phrase = Node;
        }

        public override void Undo()
        {
            base.Undo();
            Node.Project.Clipboard.Phrase = mPrevBlockClipBoard;
        }
    }
}
