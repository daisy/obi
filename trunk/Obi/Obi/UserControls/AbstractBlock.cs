using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// Type agnostic abstract base for all blocks occuring in strips
    /// </summary>
    /// //added my mg 20060804
    /// // made unabstract as this fucks up the designer :( JQ
    public /*abstract*/ class AbstractBlock: UserControl
    {
        protected PhraseNode mNode;            // the node for this block
        protected SectionStrip mSectionStrip;  // the SectionStrip that contains this block
        protected StripManagerPanel mManager;  // the manager for this block
        protected bool mSelected;              // true if the block is selected

        /// <summary>
        /// The node for this block.
        /// </summary>
        public virtual PhraseNode Node
        {
            get { return mNode; }
            set { mNode = value; }
        }

        /// <summary>
        /// The section strip in which the block appears.
        /// </summary>
        public SectionStrip SectionStrip
        {
            get { return mSectionStrip; }
            set { mSectionStrip = value; }
        }

        /// <summary>
        /// The strip manager.
        /// </summary>
        public virtual StripManagerPanel Manager
        {
            get { return mManager; }
            set { mManager = value; }
        }

        /// <summary>
        /// True if the block is currently selected.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                if (mNode != null) if (mSelected) MarkSelected(); else MarkDeselected();
            }
        }

        public AbstractBlock()
        {
        }

        /// <summary>
        /// Mark the block as being selected.
        /// </summary>
        internal virtual void MarkSelected()
        {
            mSelected = true;
        }

        /// <summary>
        /// Mark the block as being deselected.
        /// </summary>
        internal virtual void MarkDeselected()
        {
            mSelected = false;
        }
    }
}
