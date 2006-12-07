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
    /// // made unabastract as this fucks up the designer :( JQ
    public /*abstract*/ class AbstractBlock: UserControl
    {
        protected PhraseNode mNode;                 // the node for this block
        protected SectionStrip mSectionStrip;     // the SectionStrip that contains this block
        protected StripManagerPanel mManager;     // the manager for this block

        #region properties

        //md 20061201 i think it's safe to change this from corenode to phrasenode
        public virtual PhraseNode Node
        {
            get { return mNode; }
            set { mNode = value; }
        }

        public SectionStrip SectionStrip
        {
            get { return mSectionStrip; }
            set { mSectionStrip = value; }
        }

        public virtual StripManagerPanel Manager
        {
            get { return mManager; }
            set { mManager = value; }
        }

        #endregion

        #region instantiators
        
        public AbstractBlock()
        {
        }

        #endregion

        internal virtual void MarkSelected()
        {
        }

        internal virtual void MarkDeselected()
        {
        }

    }
}
