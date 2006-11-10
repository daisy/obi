using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Project clipboard. Can store a section node, a phrase node
    /// (for the moment both are core nodes but this will change)
    /// page number (TODO) or text annotation (TODO).
    /// </summary>
    public class Clipboard
    {
        private CoreNode mSection;
        private CoreNode mPhrase;

        /// <summary>
        /// Anything that the clipboard can contain. 
        /// </summary>
        public object Data
        {
            get { return mSection != null ? mSection : mPhrase != null ? mPhrase : null; }
            set
            {
                Clear();
                CoreNode node = value as CoreNode;
                if (Project.GetNodeType(node) == NodeType.Section)
                {
                    Section = node;
                }
                else if (Project.GetNodeType(node) == NodeType.Phrase)
                {
                    Phrase = node;
                }
            }
        }

        /// <summary>
        /// The section node in the clipboard.
        /// </summary>
        public CoreNode Section
        {
            get { return mSection; }
            set
            {
                Clear();
                mSection = value;
            }
        }

        /// <summary>
        /// The phrase node in the clipboard.
        /// </summary>
        public CoreNode Phrase
        {
            get { return mPhrase; }
            set
            {
                Clear();
                mPhrase = value;
            }
        }

        /// <summary>
        /// Create a new, empty clipboard.
        /// </summary>
        public Clipboard()
        {
            Clear();
        }

        /// <summary>
        /// Remove all data from the clipboard.
        /// </summary>
        public void Clear()
        {
            mSection = null;
            mPhrase = null;
        }
    }
}
