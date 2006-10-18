namespace Obi.Events.Node.Section
{
    public delegate void RequestToRenameSectionNodeHandler(object sender, RenameEventArgs e);
    public delegate void RenamedSectionNodeHandler(object sender, RenameEventArgs e);

    /// <summary>
    /// Renaming a section.
    /// </summary>
    public class RenameEventArgs: Obi.Events.Node.Section.EventArgs
    {
        private string mNewLabel;  // new label

        /// <summary>
        /// The new label for the section.
        /// </summary>
        public string NewLabel
        {
            get { return mNewLabel; }
        }

        /// <summary>
        /// Create a new RenameEventArgs object for a node and its new label.
        /// </summary>
        public RenameEventArgs(object origin, SectionNode node, string newLabel)
            : base(origin, node)
        {
            mNewLabel = newLabel;
        }
    }
}
