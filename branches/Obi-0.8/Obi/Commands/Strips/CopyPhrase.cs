using urakawa.core;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// Copy a phrase node.
    /// </summary>
    class CopyPhrase: Command
    {
        private Project mProject;    // current project
        private CoreNode mNode;      // current node
        private object mPrevData;    // previous data in the clipboard

        public override string Label
        {
            get { return Localizer.Message("copy_phrase_command_label"); }
        }

        /// <summary>
        /// Create a new copy phrase command.
        /// </summary>
        /// <param name="project">Current project.</param>
        /// <param name="node">The node to copy.</param>
        public CopyPhrase(Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mPrevData = mProject.Clipboard.Data;
        }

        /// <summary>
        /// Put the phrase node in the clipboard.
        /// </summary>
        public override void Do()
        {
            mProject.Clipboard.Phrase = mNode;
        }

        /// <summary>
        /// Restore the data in the clipboard.
        /// </summary>
        public override void Undo()
        {
            mProject.Clipboard.Data = mPrevData;
        }
    }
}
