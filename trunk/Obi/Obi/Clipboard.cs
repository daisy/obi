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
        private SectionNode mSection;  // a section node
        private PhraseNode mPhrase;    // a phrase node
        private bool mDeep;            // whether the section subtree is copied or not

        /// <summary>
        /// Anything that the clipboard can contain. 
        /// When setting a section, the default is to set as "strip", so use Section.set instead
        /// from the TOC panel.
        /// </summary>
        public object Data
        {
            get { return mSection != null ? (CoreNode)mSection : mPhrase != null ? (CoreNode)mPhrase : null; }
            set
            {
                Clear();
                CoreNode node = value as CoreNode;
                if (node.GetType() == System.Type.GetType("Obi.SectionNode"))
                {
                    Strip = (SectionNode)node;
                }
                else if (node.GetType() == System.Type.GetType("Obi.PhraseNode"))
                {
                    Phrase = (PhraseNode)node;
                }
            }
        }

        /// <summary>
        /// The section node in the clipboard.
        /// </summary>
        public SectionNode Section
        {
            get { return mSection; }
            set
            {
                Clear();
                mSection = value;
                mDeep = true;
            }
        }

        /// <summary>
        /// Same as section, except that it will always be shallow pasted.
        /// Use the Section getter if you want to look at the strip.
        /// </summary>
        public SectionNode Strip
        {
            set
            {
                Clear();
                mSection = value;
                mDeep = false;
            }
        }

        /// <summary>
        /// The phrase node in the clipboard.
        /// </summary>
        public PhraseNode Phrase
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
