using System;
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

        /// <summary>
        /// Anything that the clipboard can contain. 
        /// </summary>
        public object Data
        {
            get { return mSection != null ? (object)mSection : mPhrase != null ? (object)mPhrase : null; }
            set
            {
                Clear();
                if (value is SectionNode)
                {
                    Section = (SectionNode)value;
                }
                else if (value is PhraseNode)
                {
                    Phrase = (PhraseNode)value;
                }
                else
                {
                    throw new Exception(string.Format("Invalid type for clipboard: {0}", value.GetType()));
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
