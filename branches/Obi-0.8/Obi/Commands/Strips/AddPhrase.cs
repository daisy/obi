using urakawa.core;
using urakawa.media;

namespace Obi.Commands.Strips
{
    public class AddPhrase: Command
    {
        protected PhraseNode mNode;   // the phrase node to add/remove
        private SectionNode mParent;  // the section node to add to
        private int mIndex;           // position within the parent

        public override string Label
        {
            get { return Localizer.Message("add_phrase_command_label"); }
        }

        public AddPhrase(PhraseNode node)
        {
            mNode = node;
            mParent = mNode.ParentSection;
            mIndex = mNode.Index;
        }

        /// <summary>
        /// Do: add the node to the project; it will send the synchronization events.
        /// This is really redo, so the node exists and so does its parent.
        /// </summary>
        public override void Do()
        {
            mNode.Project.AddPhraseNodeAndAsset(mNode, mParent, mIndex);
        }

        /// <summary>
        /// Undo: remove the node from the project; it will send the synchronization events.
        /// This node has no descendant when we undo.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.DeletePhraseNodeAndAsset(mNode);
        }
    }

    public class PastePhrase : AddPhrase
    {
        public override string Label
        {
            get { return Localizer.Message("paste_phrase_command_label");  }
        }

        public PastePhrase(PhraseNode node)
            : base(node)
        {
        }

        public override void Do()
        {
            base.Do();
            mNode.Project.SetAudioMediaAsset(mNode, mNode.Asset);
        }
    }
}
