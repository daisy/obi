using urakawa.core;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// Command to delete a phrase. Undo restores the phrase in its place.
    /// The command must be created before the node is actually deleted.
    /// </summary>
    public class DeletePhrase : Command
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

        /// <summary>
        /// Create a new command.
        /// Issue *before* deleting the node as we need its parent/index information.
        /// </summary>
        /// <param name="node">The node to be deleted.</param>
        public DeletePhrase(PhraseNode node)
        {
            mNode = node;
            mParent = node.ParentSection;
            mIndex = node.Index;
        }

        /// <summary>
        /// Do: delete the node and its asset from the tree/project.
        /// </summary>
        public override void Do()
        {
            mNode.Project.DeletePhraseNodeAndMedia(mNode);
        }

        /// <summary>
        /// Undo: readd the node and its asset in the tree/project.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.AddPhraseNodeWithAudio(mNode, mParent, mIndex);
        }
    }

    /// <summary>
    /// Cut is like delete except that the deleted node is stored in the clipboard.
    /// Previous values are saved so that undo restores the previous clipboard value.
    /// </summary>
    class CutPhrase : DeletePhrase
    {
        private object mPrevious;  // previous object on the clipboard

        public override string Label
        {
            get { return Localizer.Message("cut_phrase_command_label"); }
        }

        /// <summary>
        /// Create a new command.
        /// Use *before* cutting the node to retain the previous clipboard data.
        /// </summary>
        /// <param name="node">The node to be cut.</param>
        public CutPhrase(PhraseNode node)
            : base(node)
        {
            mPrevious = node.Project.Clipboard.Data;
        }

        /// <summary>
        /// Do: delete the node again and put it back in the clipboard.
        /// </summary>
        public override void Do()
        {
            base.Do();
            Node.Project.Clipboard.Phrase = Node;
        }

        /// <summary>
        /// Undo: restore the node and the previous clipboard data.
        /// </summary>
        public override void Undo()
        {
            base.Undo();
            Node.Project.Clipboard.Data = mPrevious;
        }
    }
}
