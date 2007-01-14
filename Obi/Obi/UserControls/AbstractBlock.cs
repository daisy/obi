using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// Type agnostic abstract base for all blocks occuring in strips
    /// </summary>
    public /*abstract*/ class AbstractBlock: UserControl
    {
        protected PhraseNode mNode;            // should be more generic
        protected SectionStrip mSectionStrip;  // the SectionStrip that contains this block
        protected StripManagerPanel mManager;  // the manager for this block
        protected bool mSelected;              // true if the block is selected

        /// <summary>
        /// The node associated with the block.
        /// </summary>
        public /*abstract*/ virtual PhraseNode Node
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
        public virtual bool Selected
        {
            get { return mSelected; }
            set { mSelected = value; }
        }

        /// <summary>
        /// True if the block (or rather its node) is used.
        /// Update the widget to reflect the change.
        /// </summary>
        public virtual /*abstract*/ bool Used
        {
            set { }
        }

        /// <summary>
        /// Initialize the members of the block.
        /// </summary>
        public AbstractBlock()
        {
            mNode = null;
            mManager = null;
            mSectionStrip = null;
            Selected = false;
        }

        /// <summary>
        /// A corresponding block was selected.
        /// </summary>
        internal virtual void AlignSelected()
        {
            // Size = new Size(Width + Colors.SelectionWidth * 2, Height);
            // Padding = new Padding(Colors.SelectionWidth);
        }

        /// <summary>
        /// A corresponding block was deselected.
        /// </summary>
        internal virtual void AlignDeselected()
        {
            // Size = new Size(Width - Colors.SelectionWidth * 2, Height);
            // Padding = new Padding(0);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AbstractBlock
            // 
            this.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.Name = "AbstractBlock";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.ResumeLayout(false);

        }
    }
}
